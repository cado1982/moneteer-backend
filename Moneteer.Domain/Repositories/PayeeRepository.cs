using Dapper;
using Microsoft.Extensions.Logging;
using Moneteer.Domain.Entities;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Sql;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Moneteer.Domain.Repositories
{
    public class PayeeRepository : BaseRepository<PayeeRepository>, IPayeeRepository
    {
        public PayeeRepository(ILogger<PayeeRepository> logger)
            : base(logger)
        {

        }

        public async Task<Payee> CreatePayee(Payee payee, IDbConnection connection)
        {
            try
            {
                payee.Id = Guid.NewGuid();

                var parameters = new DynamicParameters();

                parameters.Add("@Id", payee.Id);
                parameters.Add("@Name", payee.Name);
                parameters.Add("@BudgetId", payee.BudgetId);

                await connection.ExecuteAsync(PayeeSql.Create, parameters).ConfigureAwait(false);

                return payee;
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error creating payee");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating payee");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task DeletePayee(Guid payeeId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Id", payeeId);

                await connection.ExecuteAsync(PayeeSql.Delete, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error deleting payee");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting payee");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<List<Payee>> GetAllForBudget(Guid budgetId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);

                var payees = await connection.QueryAsync<Payee>(PayeeSql.GetForBudget, parameters).ConfigureAwait(false);

                return payees.ToList();
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error getting budget payees");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting budget payees");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<Guid> GetOwner(Guid payeeId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@PayeeId", payeeId);

                return await connection.ExecuteScalarAsync<Guid>(PayeeSql.GetOwner, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error getting payee owner");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting payee owner");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<Payee> GetPayee(Guid payeeId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Id", payeeId);

                return await connection.QuerySingleOrDefaultAsync<Payee>(PayeeSql.Get, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error getting payee");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting payee");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task UpdatePayee(Payee payee, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Id", payee.Id);
                parameters.Add("@Name", payee.Name);

                await connection.ExecuteAsync(PayeeSql.Update, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error updating payee");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating payee");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }
    }
}
