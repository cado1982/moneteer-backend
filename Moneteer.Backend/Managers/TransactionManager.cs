﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moneteer.Backend.Extensions;
using Moneteer.Domain.Guards;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using Moneteer.Models;
using Moneteer.Models.Validation;

namespace Moneteer.Backend.Managers
{
    public class TransactionManager : BaseManager, ITransactionManager
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionAssignmentRepository _transactionAssignmentRepository;
        private readonly IPayeeRepository _payeeRepository;
        private readonly TransactionValidationStrategy _validationStrategy;
        private readonly IConnectionProvider _connectionProvider;
        private readonly IEnvelopeRepository _envelopeRepository;
        private readonly IBudgetRepository _budgetRepository;

        public TransactionManager(
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            ITransactionAssignmentRepository transactionAssignmentRepository,
            IPayeeRepository payeeRepository,
            IEnvelopeRepository envelopeRepository,
            IBudgetRepository budgetRepository,
            TransactionValidationStrategy validationStrategy,
            Guards guards,
            IConnectionProvider connectionProvider)
            : base(guards)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _transactionAssignmentRepository = transactionAssignmentRepository;
            _payeeRepository = payeeRepository;
            _validationStrategy = validationStrategy;
            _connectionProvider = connectionProvider;
            _envelopeRepository = envelopeRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction, Guid userId)
        {
            await GuardAccount(transaction.Account.Id, userId).ConfigureAwait(false);

            _validationStrategy.RunRules(transaction);

            var transactionEntity = transaction.ToEntity();

            using (var conn = _connectionProvider.GetOpenConnection())
            using (var dbTransaction = conn.BeginTransaction())
            {
                var account = await _accountRepository.Get(transactionEntity.Account.Id, conn).ConfigureAwait(false);
                transactionEntity.Account = account ?? throw new ApplicationException("Account not found");

                if (transactionEntity.Payee != null)
                {
                    var payee = await _payeeRepository.GetPayee(account.BudgetId, transactionEntity.Payee.Name, conn).ConfigureAwait(false);

                    if (payee == null && !String.IsNullOrWhiteSpace(transactionEntity.Payee.Name))
                    {
                        transactionEntity.Payee.BudgetId = account.BudgetId;
                        payee = await _payeeRepository.CreatePayee(transactionEntity.Payee, conn).ConfigureAwait(false);
                    }

                    transactionEntity.Payee = payee;
                }

                transactionEntity = await _transactionRepository.CreateTransaction(transactionEntity, conn).ConfigureAwait(false);

                await _transactionAssignmentRepository.CreateTransactionAssignments(transactionEntity.Assignments, transactionEntity.Id, conn).ConfigureAwait(false);

                // Adjust envelope balances
                foreach (var assignment in transaction.Assignments)
                {
                    await _envelopeRepository.AdjustBalance(assignment.Envelope.Id, assignment.Inflow - assignment.Outflow, conn).ConfigureAwait(false);
                }

                // Adjust budget available balance
                if (transaction.Inflow > 0)
                {
                    await _budgetRepository.AdjustAvailable(account.BudgetId, transaction.Inflow, conn).ConfigureAwait(false);
                }

                dbTransaction.Commit();
            }
            return transactionEntity.ToModel();
        }

        public async Task DeleteTransactions(List<Guid> transactionIds, Guid userId)
        {
            await GuardTransactions(transactionIds, userId).ConfigureAwait(false);

            using (var conn = _connectionProvider.GetOpenConnection())
            using (var dbTransaction = conn.BeginTransaction())
            {
                var transactions = await _transactionRepository.GetByIds(transactionIds, conn).ConfigureAwait(false);

                // Make sure we're only dealing with one budget
                var budgetId = transactions.Select(t => t.Account.BudgetId).Distinct().SingleOrDefault();
                if (budgetId == Guid.Empty)
                {
                    throw new InvalidOperationException("Budget could not be found for transaction");
                }

                var assignments = transactions.SelectMany(t => t.Assignments);
                var groupedByEnvelope = assignments.GroupBy(a => a.Envelope.Id);

                foreach (var envelope in groupedByEnvelope)
                {
                    var adjustment = envelope.Sum(e => e.Outflow - e.Inflow);
                    await _envelopeRepository.AdjustBalance(envelope.Key, adjustment, conn).ConfigureAwait(false);
                }

                await _transactionRepository.DeleteTransactions(transactionIds, conn).ConfigureAwait(false);

                var inflows = transactions.Where(t => t.Inflow > 0).Sum(t => t.Inflow);
                await _budgetRepository.AdjustAvailable(budgetId, -inflows, conn).ConfigureAwait(false);

                dbTransaction.Commit();
            }
        }

        public async Task<List<Transaction>> GetAllForAccount(Guid accountId, Guid userId)
        {
            await GuardAccount(accountId, userId).ConfigureAwait(false);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                var transactions = await _transactionRepository.GetAllForAccount(accountId, conn).ConfigureAwait(false);

                return transactions.ToModels().ToList();
            }
        }

        public async Task<List<Transaction>> GetAllForBudget(Guid budgetId, Guid userId)
        {
            await GuardBudget(budgetId, userId).ConfigureAwait(false);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                var transactions = await _transactionRepository.GetAllForBudget(budgetId, conn).ConfigureAwait(false);

                return transactions.ToModels().ToList();
            }
        }

        public async Task SetTransactionIsCleared(Guid transactionId, bool isCleared, Guid userId)
        {
            await GuardTransaction(transactionId, userId).ConfigureAwait(false);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await _transactionRepository.SetIsCleared(transactionId, isCleared, conn).ConfigureAwait(false);
            }
        }

        public async Task<Transaction> UpdateTransaction(Transaction transaction, Guid userId)
        {
            await GuardTransaction(transaction.Id, userId).ConfigureAwait(false);

            _validationStrategy.RunRules(transaction);

            return transaction;
        }
    }
}