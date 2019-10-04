using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moneteer.Backend.Extensions;
using Moneteer.Domain.Entities.Comparers;
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
            Guards guards,
            IConnectionProvider connectionProvider)
            : base(guards)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _transactionAssignmentRepository = transactionAssignmentRepository;
            _payeeRepository = payeeRepository;
            _connectionProvider = connectionProvider;
            _envelopeRepository = envelopeRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardAccount(transaction.Account.Id, userId, conn).ConfigureAwait(false);

                var transactionEntity = transaction.ToEntity();

                using (var dbTransaction = conn.BeginTransaction())
                {
                    var account = await _accountRepository.Get(transactionEntity.Account.Id, conn).ConfigureAwait(false);

                    if (account == null) {
                        throw new ApplicationException("Account does not exist");
                    }

                    transactionEntity.Account = account;

                    if (transactionEntity.Payee?.Id == Guid.Empty)
                    {
                        transactionEntity.Payee.BudgetId = account.BudgetId;
                        var newPayee = await _payeeRepository.CreatePayee(transactionEntity.Payee, conn).ConfigureAwait(false);

                        transactionEntity.Payee = newPayee;
                    }

                    transactionEntity = await _transactionRepository.CreateTransaction(transactionEntity, conn).ConfigureAwait(false);

                    await _transactionAssignmentRepository.CreateTransactionAssignments(transactionEntity.Assignments, transactionEntity.Id, conn).ConfigureAwait(false);
                    
                    dbTransaction.Commit();
                }
                return transactionEntity.ToModel();
            }

        }

        public async Task DeleteTransactions(List<Guid> transactionIds, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            using (var dbTransaction = conn.BeginTransaction())
            {
                await GuardTransactions(transactionIds, userId, conn).ConfigureAwait(false);
            
                var transactions = await _transactionRepository.GetByIds(transactionIds, conn).ConfigureAwait(false);

                await _transactionRepository.DeleteTransactions(transactionIds, conn).ConfigureAwait(false);

                dbTransaction.Commit();
            }
        }

        public async Task<List<Transaction>> GetAllForAccount(Guid accountId, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardAccount(accountId, userId, conn).ConfigureAwait(false);

                var transactions = await _transactionRepository.GetAllForAccount(accountId, conn).ConfigureAwait(false);

                return transactions.ToModels().ToList();
            }
        }

        public async Task<List<Transaction>> GetAllForBudget(Guid budgetId, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardBudget(budgetId, userId, conn).ConfigureAwait(false);
            
                var transactions = await _transactionRepository.GetAllForBudget(budgetId, conn).ConfigureAwait(false);

                return transactions.ToModels().ToList();
            }
        }

        public async Task SetTransactionIsCleared(Guid transactionId, bool isCleared, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardTransaction(transactionId, userId, conn).ConfigureAwait(false);
            
                await _transactionRepository.SetIsCleared(transactionId, isCleared, conn).ConfigureAwait(false);
            }
        }

        public async Task<Transaction> UpdateTransaction(Transaction transaction, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            using (var dbTrans = conn.BeginTransaction())
            {
                await GuardTransaction(transaction.Id, userId, conn).ConfigureAwait(false);

                var newTransaction = transaction.ToEntity();

                var existingTransaction = await _transactionRepository.GetById(transaction.Id, conn).ConfigureAwait(false);

                if (existingTransaction == null)
                {
                    throw new ApplicationException("Transaction not found");
                }

                // Delete all the transaction assignments
                await _transactionAssignmentRepository.DeleteTransactionAssignmentsByTransactionId(transaction.Id, conn);
                // Then just recreate them
                await _transactionAssignmentRepository.CreateTransactionAssignments(newTransaction.Assignments, transaction.Id, conn);

                await _transactionRepository.UpdateTransaction(newTransaction, conn);

                dbTrans.Commit();

                var trans = await _transactionRepository.GetById(transaction.Id, conn);

                return trans.ToModel();
            }
        }

        public async Task<List<RecentTransactionByEnvelope>> GetRecentTransactionsByEnvelope(Guid budgetId, int numberOfTransactions, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardBudget(budgetId, userId, conn).ConfigureAwait(false);
                
                var transactions = await _transactionRepository.GetRecentTransactionsByEnvelope(budgetId, numberOfTransactions, conn);

                var models = transactions.ToModels();

                return models.ToList();
            }
        }
    }
}
