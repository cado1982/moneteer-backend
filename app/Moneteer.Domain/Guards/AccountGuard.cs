using Moneteer.Domain.Exceptions;
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

        public AccountGuard(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task Guard(Guid accountId, Guid userId, IDbConnection conn)
        {
            if (accountId == Guid.Empty) throw new ArgumentException("accountId must be provided", nameof(accountId));
            if (userId == Guid.Empty) throw new ArgumentException("userId must be provided", nameof(userId));

            var accountOwnerId = await _accountRepository.GetOwner(accountId, conn);

            if (accountOwnerId != userId)
            {
                throw new ForbiddenException($"User {userId} does not have access to account {accountId}");
            }
        }
    }
}
