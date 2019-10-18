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
                if (ex.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    throw new ApplicationException("Envelope name already exists.");
                }

                LogPostgresException(ex, $"Error creating envelope for category: {envelope.EnvelopeCategory.Id}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error creating envelope for category: {envelope.EnvelopeCategory.Id}");
                throw;
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
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error creating default envelopes for budget: {budgetId}");
                throw;
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
                if (ex.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    throw new ApplicationException("Envelope category name already exists.");
                }

                LogPostgresException(ex, $"Error creating envelope category for budget: {budgetId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error creating envelope category for budget: {budgetId}");
                throw;
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
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting envelopes for budget: {budgetId}");
                throw;
            }
        }

        public async Task AdjustAssigned(Guid envelopeId, decimal assignedAdjustment, IDbConnection conn)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@EnvelopeId", envelopeId);
                parameters.Add("@Adjustment", assignedAdjustment);

                await conn.ExecuteAsync(EnvelopeSql.AdjustAssigned, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error adjusting assigned by {assignedAdjustment} for envelope: {envelopeId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adjusting assigned by {assignedAdjustment} for envelope: {envelopeId}");
                throw;
            }
        }

        public async Task DeleteEnvelope(Guid envelopeId, IDbConnection conn)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@EnvelopeId", envelopeId);

                await conn.ExecuteAsync(EnvelopeSql.DeleteEnvelope, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == PostgresErrorCodes.ForeignKeyConstraintViolation)
                {
                    throw new ApplicationException("Envelope cannot be deleted because there are transactions using it.");
                }

                LogPostgresException(ex, $"Error deleting envelope: {envelopeId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting envelope: {envelopeId}");
                throw;
            }
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
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting envelope categories for budget: {budgetId}");
                throw;
            }
        }

        public async Task<Guid> GetEnvelopeOwner(Guid envelopeId, IDbConnection conn)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@EnvelopeId", envelopeId);

                return await conn.QuerySingleAsync<Guid>(EnvelopeSql.GetEnvelopeOwner, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting owner for envelope: {envelopeId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting owner for envelope: {envelopeId}");
                throw;
            }
        }

        public async Task<Guid> GetEnvelopeCategoryOwner(Guid envelopeCategoryId, IDbConnection conn)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@EnvelopeCategoryId", envelopeCategoryId);

                return await conn.ExecuteScalarAsync<Guid>(EnvelopeSql.GetEnvelopeCategoryOwner, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting owner for envelope category: {envelopeCategoryId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting owner for envelope category: {envelopeCategoryId}");
                throw;
            }
        }

        public async Task<List<EnvelopeBalance>> GetEnvelopeBalances(Guid budgetId, IDbConnection conn)
        {
            if (budgetId == Guid.Empty) throw new ArgumentException("Budget Id must be provided");

            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);

                var result = await conn.QueryAsync<EnvelopeBalance>(EnvelopeSql.GetEnvelopeBalances, parameters).ConfigureAwait(false);

                return result.ToList();
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting envelope balances for budget: {budgetId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting envelope balances for budget: {budgetId}");
                throw;
            }
        }

        public async Task MoveEnvelopeBalanceMultiple(Guid fromEnvelopeId, List<Tuple<Guid, decimal>> targets, IDbConnection conn)
        {
            if (fromEnvelopeId == Guid.Empty) throw new ArgumentException("fromEnvelopeId must be provided");
            if (targets == null) throw new ArgumentException("targets must be provided");
            if (targets.Any(t => t.Item1 == Guid.Empty)) throw new ArgumentException("Target envelopeids must be provided");
            if (targets.Any(t => t.Item2 <= 0)) throw new ArgumentException("Target amounts must be greater than zero");

            try
            {
                var fromEnvelopeDeduction = -targets.Sum(t => t.Item2);

                var fromParameters = new DynamicParameters();
                fromParameters.Add("@EnvelopeId", fromEnvelopeId);
                fromParameters.Add("@Adjustment", fromEnvelopeDeduction);
                await conn.ExecuteAsync(EnvelopeSql.AdjustAssigned, fromParameters).ConfigureAwait(false);

                foreach (var target in targets)
                {
                    var toParameters = new DynamicParameters();
                    toParameters.Add("@EnvelopeId", target.Item1);
                    toParameters.Add("@Adjustment", target.Item2);
                    await conn.ExecuteAsync(EnvelopeSql.AdjustAssigned, toParameters).ConfigureAwait(false);
                }
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error moving multiple balances from envelope {fromEnvelopeId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error moving multiple balances from envelope {fromEnvelopeId}");
                throw;
            }
        }

        public async Task UpdateEnvelope(Envelope envelope, IDbConnection conn)
        {
            if (conn == null) throw new ArgumentNullException(nameof(conn));
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));
            if (envelope.Id == Guid.Empty) throw new ArgumentException("Envelope id must be provided", nameof(envelope));
            if (envelope.EnvelopeCategory == null) throw new ArgumentException(nameof(envelope));
            if (envelope.EnvelopeCategory.Id == Guid.Empty) throw new ArgumentException("Envelope category id must be provided", nameof(envelope));
            
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@EnvelopeId", envelope.Id);
                parameters.Add("@EnvelopeCategoryId", envelope.EnvelopeCategory.Id);
                parameters.Add("@IsHidden", envelope.IsHidden);

                var result = await conn.ExecuteAsync(EnvelopeSql.UpdateEnvelope, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error updating envelope: {envelope.Id}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating envelope: {envelope.Id}");
                throw;
            }
        }

        public async Task UpdateEnvelopeIsHidden(Guid envelopeId, bool isHidden, IDbConnection conn)
        {
            if (envelopeId == Guid.Empty) throw new ArgumentException("envelopeId must be provided", nameof(envelopeId));
            if (conn == null) throw new ArgumentNullException(nameof(conn));

            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@EnvelopeId", envelopeId);
                parameters.Add("@IsHidden", isHidden);

                var result = await conn.ExecuteAsync(EnvelopeSql.UpdateEnvelopeIsHidden, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error updating envelope is hidden: {envelopeId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating envelope is hidden: {envelopeId}");
                throw;
            }
        }

        public async Task<Envelope> GetEnvelope(Guid envelopeId, IDbConnection conn)
        {
            if (envelopeId == Guid.Empty) throw new ArgumentException("envelopeId must be provided", nameof(envelopeId));
            if (conn == null) throw new ArgumentNullException(nameof(conn));

            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@EnvelopeId", envelopeId);

                var result = await conn.QueryAsync<Envelope, EnvelopeCategory, Envelope>(EnvelopeSql.GetEnvelope, (e, ec) => {
                    e.EnvelopeCategory = ec;
                    return e;
                }, parameters).ConfigureAwait(false);

                return result.SingleOrDefault();
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting envelope: {envelopeId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting envelope: {envelopeId}");
                throw;
            }
        }

        public async Task<Guid> GetAvailableIncomeEnvelopeId(Guid budgetId, IDbConnection conn)
        {
            if (budgetId == Guid.Empty) throw new ArgumentException("envelopeId must be provided", nameof(budgetId));
            if (conn == null) throw new ArgumentNullException(nameof(conn));

            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", budgetId);

                var result = await conn.ExecuteScalarAsync<Guid>(EnvelopeSql.GetAvailableIncomeEnvelopeId, parameters).ConfigureAwait(false);

                return result;
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting available income envelope for budget: {budgetId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting available income envelope for budget: {budgetId}");
                throw;
            }
        }

        public async Task UpdateEnvelopeCategoryIsToggled(Guid envelopeCategoryId, bool isToggled, IDbConnection conn)
        {
            if (envelopeCategoryId == Guid.Empty) throw new ArgumentException("envelopeCategoryId must be provided", nameof(envelopeCategoryId));
            if (conn == null) throw new ArgumentNullException(nameof(conn));

            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@EnvelopeCategoryId", envelopeCategoryId);
                parameters.Add("@IsToggled", isToggled);

                var result = await conn.ExecuteAsync(EnvelopeSql.UpdateEnvelopeCategoryIsToggled, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error updating envelope category is toggled: {envelopeCategoryId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating envelope category is toggled: {envelopeCategoryId}");
                throw;
            }
        }
    }
}
