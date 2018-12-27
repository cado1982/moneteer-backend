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
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating transaction assignment");
                throw new ApplicationException("Oops! Something went wrong. Please try again");
            }
        }
    }


}
