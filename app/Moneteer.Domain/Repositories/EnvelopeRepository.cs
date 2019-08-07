using Dapper;
using Microsoft.Extensions.Logging;
using Moneteer.Domain.Entities;
using Moneteer.Domain.Sql;
using Moneteer.Domain.StaticData;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Moneteer.Domain.Repositories
{
    public class EnvelopeRepository : BaseRepository<EnvelopeRepository>, IEnvelopeRepository
    {
        public EnvelopeRepository(ILogger<EnvelopeRepository> logger)
            : base(logger)
        {

        }

        public async Task<Envelope> CreateEnvelope(Envelope envelope, IDbConnection conn)
        {
            if (envelope.EnvelopeCategory == null || envelope.EnvelopeCategory.Id == Guid.Empty)
            {
                throw new ArgumentException("Envelope category must be provided", nameof(envelope));
            }

            try
            {
                envelope.Id = Guid.NewGuid();

                var parameters = new DynamicParameters();

                parameters.Add("@Id", envelope.Id);
                parameters.Add("@Name", envelope.Name);
                parameters.Add("@EnvelopeCategoryId", envelope.EnvelopeCategory.Id);

                await conn.ExecuteAsync(EnvelopeSql.CreateEnvelope, parameters).ConfigureAwait(false);

                return envelope;
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error creating envelope for category: {envelope.EnvelopeCategory.Id}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error creating envelope for category: {envelope.EnvelopeCategory.Id}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<BudgetEnvelopes> CreateDefaultForBudget(Guid budgetId, IDbConnection conn)
        {
            try
            {
                var defaultCategories = DefaultCategories.GenerateDefaultCategories(budgetId);

                await conn.ExecuteAsync(EnvelopeSql.CreateEnvelopeCategory, defaultCategories.Categories).ConfigureAwait(false);

                foreach (var envelope in defaultCategories.Envelopes)
                {
                    await conn.ExecuteAsync(EnvelopeSql.CreateEnvelope, new
                    {
                        envelope.Id,
                        EnvelopeCategoryId = envelope.EnvelopeCategory.Id,
                        envelope.Name
                    }).ConfigureAwait(false);
                }

                return defaultCategories;
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error creating default envelopes for budget: {budgetId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error creating default envelopes for budget: {budgetId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<EnvelopeCategory> CreateEnvelopeCategory(Guid budgetId, EnvelopeCategory envelopeCategory, IDbConnection conn)
        {
            try
            {
                envelopeCategory.Id = Guid.NewGuid();

                var parameters = new DynamicParameters();

                parameters.Add("@Id", envelopeCategory.Id);
                parameters.Add("@Name", envelopeCategory.Name);
                parameters.Add("@BudgetId", budgetId);

                await conn.ExecuteAsync(EnvelopeSql.CreateEnvelopeCategory, parameters).ConfigureAwait(false);

                return envelopeCategory;
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error creating envelope category for budget: {budgetId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error creating envelope category for budget: {budgetId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task<List<Envelope>> GetBudgetEnvelopes(Guid budgetId, IDbConnection conn)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);

                var result = await conn.QueryAsync<Envelope, EnvelopeCategory, Envelope>(EnvelopeSql.GetEnvelopesForBudget, (envelope, category) =>
                {
                    envelope.EnvelopeCategory = category;
                    return envelope;
                }, parameters).ConfigureAwait(false);

                return result.ToList();
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting envelopes for budget: {budgetId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting envelopes for budget: {budgetId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public async Task AdjustBalance(Guid envelopeId, decimal balanceAdjustment, IDbConnection conn)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@EnvelopeId", envelopeId);
                parameters.Add("@Adjustment", balanceAdjustment);

                await conn.ExecuteAsync(EnvelopeSql.AdjustBalance, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error adjusting balance by {balanceAdjustment} for envelope: {envelopeId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adjusting balance by {balanceAdjustment} for envelope: {envelopeId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        public Task DeleteEnvelope(Guid envelopeId, IDbConnection conn)
        {
            throw new NotImplementedException();
        }

        public async Task<List<EnvelopeCategory>> GetEnvelopeCategories(Guid budgetId, IDbConnection conn)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);

                var result = await conn.QueryAsync<EnvelopeCategory>(EnvelopeSql.GetEnvelopeCategoriesForBudget, parameters).ConfigureAwait(false);

                return result.ToList();
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting envelope categories for budget: {budgetId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting envelope categories for budget: {budgetId}");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }

        //public async Task<List<EnvelopeBalance>> GetEnvelopeBalances(Guid budgetId, IDbConnection conn)
        //{
        //    try
        //    {
        //        var parameters = new DynamicParameters();

        //        parameters.Add("@BudgetId", budgetId);

        //        var result = await conn.QueryAsync<EnvelopeBalance>(EnvelopeSql.GetEnvelopeBalances, parameters).ConfigureAwait(false);

        //        return result.ToList();
        //    }
        //    catch (PostgresException ex)
        //    {
        //        LogPostgresException(ex, $"Error getting envelope balances for budget: {budgetId}");
        //        throw new ApplicationException("Oops! Something went wrong. Please try again");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex, $"Error getting envelope balances for budget: {budgetId}");
        //        throw new ApplicationException("Oops! Something went wrong. Please try again");
        //    }
        //}
    }
}
