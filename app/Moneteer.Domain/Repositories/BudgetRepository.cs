﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Moneteer.Domain.Entities;
using System;
using Npgsql;
using Microsoft.Extensions.Logging;
using Moneteer.Domain.Sql;
using System.Data;

namespace Moneteer.Domain.Repositories
{
    public class BudgetRepository : BaseRepository<BudgetRepository>, IBudgetRepository
    {
        public BudgetRepository(ILogger<BudgetRepository> logger)
            : base(logger)
        {

        }

        public async Task Create(Budget budget, IDbConnection connection)
        {
            try
            {
                budget.Id = Guid.NewGuid();

                var parameters = new DynamicParameters();

                parameters.Add("@Id", budget.Id);
                parameters.Add("@Name", budget.Name);
                parameters.Add("@UserId", budget.UserId);
                parameters.Add("@CurrencyCode", budget.CurrencyCode);
                parameters.Add("@CurrencySymbolLocation", budget.CurrencySymbolLocation);
                parameters.Add("@ThousandsSeparator", budget.ThousandsSeparator);
                parameters.Add("@DecimalSeparator", budget.DecimalSeparator);
                parameters.Add("@DecimalPlaces", budget.DecimalPlaces);
                parameters.Add("@DateFormat", budget.DateFormat);

                await connection.ExecuteAsync(BudgetSql.Create, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    throw new ApplicationException("Budget already exists");
                }

                LogPostgresException(ex, "Error creating budget");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating budget");
                throw;
            }
        }

        public async Task Delete(Guid budgetId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);

                await connection.ExecuteAsync(BudgetSql.Delete, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error deleting budget: {budgetId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting budget: {budgetId}");
                throw;
            }
        }

        public async Task<Budget> Get(Guid budgetId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Id", budgetId);

                return await connection.QuerySingleAsync<Budget>(BudgetSql.Get, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting budget: {budgetId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting budget: {budgetId}");
                throw;
            }
        }

        public async Task<List<Budget>> GetAllForUser(Guid userId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@UserId", userId);

                var result = await connection.QueryAsync<Budget>(BudgetSql.GetAllForUser, parameters).ConfigureAwait(false);

                return result.ToList();
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting budgets for user: {userId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting budgets for user: {userId}");
                throw;
            }
        }

        public async Task<Guid> GetOwner(Guid budgetId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);

                return await connection.ExecuteScalarAsync<Guid>(BudgetSql.GetOwner, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting owner for budget: {budgetId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting owner for budget: {budgetId}");
                throw;
            }
        }
    }
}
