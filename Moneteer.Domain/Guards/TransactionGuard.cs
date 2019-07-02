using Moneteer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Moneteer.Domain.Guards
{
    public class TransactionGuard
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionGuard(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public Task Guard(Guid transactionId, Guid userId, IDbConnection conn)
        {
            return Guard(new List<Guid> { transactionId }, userId, conn);
        }

        public async Task Guard(List<Guid> transactionIds, Guid userId, IDbConnection conn)
        {
            var transactionOwnerIds = await _transactionRepository.GetOwners(transactionIds, conn);

            if (!transactionOwnerIds.All(t => t == userId))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
