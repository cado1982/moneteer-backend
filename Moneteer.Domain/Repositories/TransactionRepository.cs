using Dapper;
using Microsoft.Extensions.Logging;
using Moneteer.Domain.Entities;
using Moneteer.Domain.Sql;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Moneteer.Domain.Repositories
{
    public class TransactionRepository : BaseRepository<TransactionRepository>, ITransactionRepository
    {
        public TransactionRepository(ILogger<TransactionRepository> logger)
            : base(logger)
        {

        }

        public async Task<Transaction> CreateTransaction(Transaction transaction, IDbConnection connection)
        {
            try
            {
                transaction.Id = Guid.NewGuid();

                var parameters = new DynamicParameters();

                parameters.Add("@Id", transaction.Id);
                parameters.Add("@AccountId", transaction.Account.Id);
                parameters.Add("@PayeeId", transaction.Payee == null ? (Guid?)null : transaction.Payee.Id);
                parameters.Add("@IsCleared", transaction.IsCleared);
                parameters.Add("@Date", transaction.Date);
                parameters.Add("@Description", transaction.Description);
                parameters.Add("@IsReconciled", transaction.IsReconciled);
                parameters.Add("@Inflow", transaction.Inflow);
                parameters.Add("@Outflow", transaction.Outflow);

                await connection.ExecuteAsync(TransactionSql.Create, parameters).ConfigureAwait(false);

                return transaction;
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error creating transaction");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating transaction");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task DeleteTransactions(List<Guid> transactionIds, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@TransactionIds", transactionIds);

                await connection.ExecuteAsync(TransactionSql.Delete, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error deleting transaction");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting transaction");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<List<Transaction>> GetAllForAccount(Guid accountId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@AccountId", accountId);

                return await QueryTransactions(parameters, TransactionSql.GetForAccount, connection).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error getting transactions for account");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting transactions for account");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<List<Transaction>> GetAllForBudget(Guid budgetId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);

                return await QueryTransactions(parameters, TransactionSql.GetForBudget, connection).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error getting transations for budget");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting transations for budget");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<List<Transaction>> GetBudgetOnOrBefore(Guid budgetId, short year, short month, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);
                parameters.Add("@Year", year);
                parameters.Add("@Month", month);

                return await QueryTransactions(parameters, TransactionSql.GetBudgetOnOrBefore, connection).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error getting transations for budget");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting transations for budget");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<List<Transaction>> GetByIds(List<Guid> transactionIds, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Ids", transactionIds);

                return await QueryTransactions(parameters, TransactionSql.GetByIds, connection).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error getting transations by ids");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting transations by ids");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<List<Transaction>> GetForMonth(Guid budgetId, short year, short month, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);
                parameters.Add("@Year", year);
                parameters.Add("@Month", month);

                return await QueryTransactions(parameters, TransactionSql.GetForMonth, connection).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error getting transations for budget");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting transations for budget");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<List<Transaction>> GetOnOrBefore(Guid budgetId, short year, short month, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);
                parameters.Add("@Year", year);
                parameters.Add("@Month", month);

                return await QueryTransactions(parameters, TransactionSql.GetOnOrBefore, connection).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error getting transations for budget");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting transations for budget");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<Guid> GetOwner(Guid transactionId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@TransactionId", transactionId);

                return await connection.ExecuteScalarAsync<Guid>(TransactionSql.GetOwner, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error getting owner for transaction");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting owner for transaction");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<List<Guid>> GetOwners(List<Guid> transactionIds, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@TransactionIds", transactionIds);

                var result = await connection.QueryAsync<Guid>(TransactionSql.GetOwners, parameters).ConfigureAwait(false);

                return result.ToList();
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error getting owner for transaction");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting owner for transaction");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task SetIsCleared(Guid transactionId, bool isCleared, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@TransactionId", transactionId);
                parameters.Add("@IsCleared", isCleared);

                await connection.ExecuteAsync(TransactionSql.SetIsCleared, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error setting isCleared for transaction");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error setting isCleared for transaction");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task UpdateTransaction(Transaction transaction, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Id", transaction.Id);
                parameters.Add("@AccountId", transaction.Account != null ? transaction.Account.Id : (Guid?)null);
                parameters.Add("@PayeeId", transaction.Payee != null ? transaction.Payee.Id : (Guid?)null);
                parameters.Add("@IsCleared", transaction.IsCleared);
                parameters.Add("@Date", transaction.Date);
                parameters.Add("@Description", transaction.Description);
                parameters.Add("@IsReconciled", transaction.IsReconciled);
                parameters.Add("@Inflow", transaction.Inflow);
                parameters.Add("@Outflow", transaction.Outflow);

                await connection.ExecuteAsync(TransactionSql.Update, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error updating transaction");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating transaction");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        private async Task<List<Transaction>> QueryTransactions(DynamicParameters parameters, string sql, IDbConnection connection)
        {
            var transactionDictionary = new Dictionary<Guid, Transaction>();

            var result = await connection.QueryAsync<Transaction, Account, Payee, TransactionAssignment, Envelope, Transaction>(sql, (t, a, p, ta, c) =>
            {
                if (!transactionDictionary.TryGetValue(t.Id, out Transaction transactionEntry))
                {
                    transactionEntry = t;
                    transactionEntry.Assignments = new List<TransactionAssignment>();
                    transactionEntry.Account = a;
                    transactionEntry.Payee = p;
                    transactionDictionary.Add(transactionEntry.Id, transactionEntry);
                }

                if (ta != null)
                {
                    ta.Envelope = c;
                    transactionEntry.Assignments.Add(ta);
                }

                return transactionEntry;
            }, parameters).ConfigureAwait(false);

            return result.ToList();
        }
    }
}
