﻿using Moneteer.Domain.Repositories;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Moneteer.Domain.Guards
{
    public class PayeeGuard
    {
        private readonly IPayeeRepository _payeeRepository;

        public PayeeGuard(IPayeeRepository payeeRepository)
        {
            _payeeRepository = payeeRepository;
        }

        public async Task Guard(Guid payeeId, Guid userId, IDbConnection conn)
        {
            var payeeOwnerId = await _payeeRepository.GetOwner(payeeId, conn);

            if (payeeOwnerId != userId)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}