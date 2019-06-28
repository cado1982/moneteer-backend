using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Moneteer.Domain.Guards
{
    public class AccountGuard
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConnectionProvider _connectionProvider;

        public AccountGuard(IAccountRepository accountRepository, IConnectionProvider connectionProvider)
        {
            _accountRepository = accountRepository;
            _connectionProvider = connectionProvider;
        }

        public async Task Guard(Guid accountId, Guid userId, IDbConnection conn)
        {
            var accountOwnerId = await _accountRepository.GetOwner(accountId, conn);

            if (accountOwnerId != userId)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
