using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Moneteer.Backend.Extensions;
using Moneteer.Domain.Guards;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using Moneteer.Models;
using Moneteer.Models.Validation;
using Entities = Moneteer.Domain.Entities;

namespace Moneteer.Backend.Managers
{
    public class AccountManager : BaseManager, IAccountManager
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEnvelopeRepository _envelopeRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionAssignmentRepository _transactionAssignmentRepository;
        private readonly AccountValidationStrategy _validationStrategy;
        private readonly IConnectionProvider _connectionProvider;
        private readonly IBudgetRepository _budgetRepository;

        public AccountManager(
            IBudgetRepository budgetRepository,
            IAccountRepository accountRepository,
            IEnvelopeRepository envelopeRepository,
            ITransactionRepository transactionRepository,
            ITransactionAssignmentRepository transactionAssignmentRepository,
            AccountValidationStrategy validationStrategy,
            IConnectionProvider connectionProvider,
            Guards guards)
                : base(guards)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _envelopeRepository = envelopeRepository;
            _transactionAssignmentRepository = transactionAssignmentRepository;
            _validationStrategy = validationStrategy;
            _connectionProvider = connectionProvider;
            _budgetRepository = budgetRepository;
        }

        public async Task<Account> Create(Account account, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            using (var dbTransaction = conn.BeginTransaction())
            {
                await GuardBudget(account.BudgetId, userId, conn);

                _validationStrategy.RunRules(account);

                var entity = account.ToEntity();

                await _accountRepository.Create(entity, conn);

                if (account.InitialBalance != 0)
                {
                    await CreateInitialBalanceTransaction(entity, account.InitialBalance, conn);
                }

                account = entity.ToModel();
                account.ClearedBalance = account.InitialBalance;
                account.UnclearedBalance = 0;

                dbTransaction.Commit();

                return account;
            }
        }

        private async Task CreateInitialBalanceTransaction(Entities.Account account, decimal initialBalance, IDbConnection conn)
        {
            if (initialBalance == 0) return;

            var transaction = new Entities.Transaction
            {
                Account = account,
                Date = DateTime.UtcNow.Date,
                Assignments = new List<Entities.TransactionAssignment>(),
                Inflow = 0,
                Outflow = 0,
                IsCleared = true,
                IsReconciled = true,
                Description = "Initial Balance"
            };

            if (initialBalance > 0)
            {
                transaction.Inflow = initialBalance;
                await _budgetRepository.AdjustAvailable(account.BudgetId, initialBalance, conn).ConfigureAwait(false);
            }
            else if (initialBalance < 0)
            {
                var envelopes = await _envelopeRepository.GetBudgetEnvelopes(account.BudgetId, conn);

                var preMoneteerDebtEnvelope = envelopes.SingleOrDefault(e => e.Name == "Pre Moneteer Debt");

                transaction.Outflow = Math.Abs(initialBalance);
                transaction.Assignments.Add(new Entities.TransactionAssignment{
                    Outflow = Math.Abs(initialBalance),
                    Envelope = preMoneteerDebtEnvelope
                });
            }

            var newTransaction = await _transactionRepository.CreateTransaction(transaction, conn);

            if (transaction.Assignments != null && transaction.Assignments.Any())
            {
                await _transactionAssignmentRepository.CreateTransactionAssignments(transaction.Assignments, newTransaction.Id, conn);
            }
        }

        public async Task Delete(Guid accountId, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardAccount(accountId, userId, conn);

                await _accountRepository.Delete(accountId, conn);
            }
        }

        public async Task<List<Account>> GetAllForBudget(Guid budgetId, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardBudget(budgetId, userId, conn);
            
                var entities = await _accountRepository.GetAllForBudget(budgetId, conn);

                var accountBalances = await _accountRepository.GetAccountBalances(budgetId, conn);

                var models = entities.Select(e => e.ToModel()).ToList();

                foreach (var accountBalance in accountBalances)
                {
                    var accountModel = models.SingleOrDefault(a => a.Id == accountBalance.AccountId);

                    if (accountModel != null)
                    {
                        accountModel.ClearedBalance = accountBalance.ClearedBalance;
                        accountModel.UnclearedBalance = accountBalance.UnclearedBalance;
                    }
                }

                return models;
            }
        }

        public async Task Update(Account account, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardAccount(account.Id, userId, conn);

                _validationStrategy.RunRules(account);

                var entity = new Entities.Account
                {
                    Id = account.Id,
                    Name = account.Name,
                    IsBudget = account.IsBudget
                };

                await _accountRepository.Update(entity, conn);
            }
        }

        public async Task<Account> Get(Guid accountId, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardAccount(accountId, userId, conn);

                var entity = await _accountRepository.Get(accountId, conn);

                var accountBalance = await _accountRepository.GetAccountBalance(accountId, conn);

                var model = entity.ToModel();

                if (accountBalance != null)
                {
                    model.ClearedBalance = accountBalance.ClearedBalance;
                    model.UnclearedBalance = accountBalance.UnclearedBalance;
                }

                return model;
            }
        }
    }
}
