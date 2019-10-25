using Dapper;
using Microsoft.Extensions.Logging;
using Moneteer.Domain.Entities;
using Moneteer.Domain.Sql;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Moneteer.Domain.Repositories
{
    public class TransactionAssignmentRepository : BaseRepository<TransactionAssignmentRepository>, ITransactionAssignmentRepository
    {
        public TransactionAssignmentRepository(ILogger<TransactionAssignmentRepository> logger)
            : base(logger)
        {

        }
        
        public async Task CreateTransactionAssignments(IEnumerable<TransactionAssignment> transactionAssignments, Guid transactionId, IDbConnection connection)
        {
            try
            {
                foreach (var assignment in transactionAssignments)
                {
                    assignment.Id = Guid.NewGuid();

                    await connection.ExecuteAsync(TransactionAssignmentSql.Create, new
                    {
                        Id = assignment.Id,
                        TransactionId = transactionId,
                        EnvelopeId = assignment.Envelope?.Id,
                        Inflow = assignment.Inflow,
                        Outflow = assignment.Outflow
                    }).ConfigureAwait(false);
                }
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error creating transaction assignment");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating transaction assignment");
                throw;
            }
        }

        public async Task DeleteTransactionAssignmentsById(IEnumerable<Guid> transactionAssignmentIds, IDbConnection connection)
        {
            try
            {
                foreach (var assignmentId in transactionAssignmentIds)
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@AssignmentId", assignmentId);

                    await connection.ExecuteAsync(TransactionAssignmentSql.DeleteByIds, parameters).ConfigureAwait(false);
                }
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, "Error deleting transaction assignments");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting transaction assignments");
                throw;
            }
        }

        public async Task DeleteTransactionAssignmentsByTransactionId(Guid transactionId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@TransactionId", transactionId);

                await connection.ExecuteAsync(TransactionAssignmentSql.DeleteByTransactionId, parameters).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error deleting transaction assignments by transaction {transactionId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting transaction assignments by transaction {transactionId}");
                throw;
            }
        }
    }
}
