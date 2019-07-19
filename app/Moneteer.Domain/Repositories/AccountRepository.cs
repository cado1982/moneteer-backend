using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Moneteer.Domain.Entities;
using Moneteer.Domain.Sql;
using Npgsql;

namespace Moneteer.Domain.Repositories
{
    public class AccountRepository : BaseRepository<AccountRepository>, IAccountRepository
    {
        public AccountRepository(ILogger<AccountRepository> logger)
            : base(logger)
        {

        }

        //public async Task AdjustBalance(Guid accountId, decimal amount, IDbConnection connection)
        //{
        //    try
        //    {
        //        var parameters = new DynamicParameters();

        //        parameters.Add("@AccountId", accountId);
        //        parameters.Add("@Adjustment", amount, DbType.Decimal);

        //        await connection.ExecuteAsync(AccountSql.AdjustBalance, parameters).ConfigureAwait(false);
        //    }
        //    catch (PostgresException ex)
        //    {
        //        LogPostgresException(ex, $"Error changing balance amount for account: {accountId}");
        //        throw new ApplicationException("Oops! Something went wrong. Please try again");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex, $"Error changing balance amount for account: {accountId}");
        //        throw new ApplicationException("Oops! Something went wrong. Please try again");
        //    }
        //}

        

        public async Task Create(Account account, IDbConnection connection)
        {
            try
            {
                account.Id = Guid.NewGuid();

                var parameters = new DynamicParameters();

                parameters.Add("@Id", account.Id);
                parameters.Add("@Name", account.Name);
                parameters.Add("@BudgetId", account.BudgetId);
                parameters.Add("@IsBudget", true /* account.IsBudget */); // All accounts are budget accounts at the moment

                await connection.ExecuteAsync(AccountSql.Create, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    throw new ApplicationException("Account already exists");
                }
                else
                {
                    LogPostgresException(ex, "Error creating account");
                    throw new ApplicationException("Oops! Something went wrong. Please try again");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating account");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task Delete(Guid accountId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@AccountId", accountId);

                await connection.ExecuteAsync(AccountSql.Delete, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error deleting account: {accountId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting account: {accountId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<Account> Get(Guid accountId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@AccountId", accountId);

                return await connection.QuerySingleAsync<Account>(AccountSql.Get, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting account: {accountId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting account: {accountId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<AccountBalance> GetAccountBalance(Guid accountId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@AccountId", accountId);

                return await connection.QuerySingleAsync<AccountBalance>(AccountSql.GetAccountBalance, parameters).ConfigureAwait(false);

            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting account balance for account: {accountId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting account balance for account: {accountId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<List<AccountBalance>> GetAccountBalances(Guid budgetId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);

                var result = await connection.QueryAsync<AccountBalance>(AccountSql.GetAccountBalances, parameters).ConfigureAwait(false);

                return result.ToList();
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting account balances for budget: {budgetId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting account balances for budget: {budgetId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<List<Account>> GetAllForBudget(Guid budgetId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);

                var result = await connection.QueryAsync<Account>(AccountSql.GetAllForBudget, parameters).ConfigureAwait(false);

                return result.ToList();
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting accounts for budget: {budgetId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting accounts for budget: {budgetId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

       

        public async Task<Guid> GetOwner(Guid accountId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@AccountId", accountId);

                return await connection.ExecuteScalarAsync<Guid>(AccountSql.GetOwner, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting owner for account: {accountId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting owner for account: {accountId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }



        public async Task Update(Account account, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@AccountId", account.Id);
                parameters.Add("@Name", account.Name);
                parameters.Add("@IsBudget", account.IsBudget);

                await connection.ExecuteAsync(AccountSql.Update, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error updating account: {account?.Id}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating account: {account?.Id}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

    }
}
