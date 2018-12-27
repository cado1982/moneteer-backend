using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Moneteer.Domain.Guards
{
    public class PayeeGuard
    {
        private readonly IPayeeRepository _payeeRepository;
        private readonly IConnectionProvider _connectionProvider;

        public PayeeGuard(IPayeeRepository payeeRepository, IConnectionProvider connectionProvider)
        {
            _payeeRepository = payeeRepository;
            _connectionProvider = connectionProvider;
        }

        public async Task Guard(Guid payeeId, Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            {
                var payeeOwnerId = await _payeeRepository.GetOwner(payeeId, conn);

                if (payeeOwnerId != userId)
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
    }
}
