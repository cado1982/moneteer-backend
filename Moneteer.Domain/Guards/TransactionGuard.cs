using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moneteer.Domain.Guards
{
    public class TransactionGuard
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IConnectionProvider _connectionProvider;

        public TransactionGuard(ITransactionRepository transactionRepository, IConnectionProvider connectionProvider)
        {
            _transactionRepository = transactionRepository;
            _connectionProvider = connectionProvider;
        }

        public Task Guard(Guid transactionId, Guid userId)
        {
            return Guard(new List<Guid> { transactionId }, userId);
        }

        public async Task Guard(List<Guid> transactionIds, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            {
                var transactionOwnerIds = await _transactionRepository.GetOwners(transactionIds, conn);

                if (!transactionOwnerIds.All(t => t == userId))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
    }
}
