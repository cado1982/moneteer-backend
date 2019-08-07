using Moneteer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Moneteer.Domain.Repositories
{
    public interface ITransactionAssignmentRepository
    {
        Task CreateTransactionAssignments(IEnumerable<TransactionAssignment> transactionAssignments, Guid transactionId, IDbConnection connection);
        Task DeleteTransactionAssignmentsById(IEnumerable<Guid> transactionAssignmentIds, IDbConnection connection);
        Task DeleteTransactionAssignmentsByTransactionId(Guid transactionId, IDbConnection connection);

    }
}
