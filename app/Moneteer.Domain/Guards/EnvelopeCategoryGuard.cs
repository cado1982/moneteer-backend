using Moneteer.Domain.Exceptions;
using Moneteer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Moneteer.Domain.Guards
{
    public class EnvelopeCategoryGuard
    {
        private readonly IEnvelopeRepository _envelopeRepository;

        public EnvelopeCategoryGuard(IEnvelopeRepository envelopeRepository)
        {
            _envelopeRepository = envelopeRepository;
        }

        public async Task Guard(Guid envelopeCategoryId, Guid userId, IDbConnection conn)
        {
            if (envelopeCategoryId == Guid.Empty) throw new ArgumentException("envelopeCategoryId must be provided", nameof(envelopeCategoryId));
            if (userId == Guid.Empty) throw new ArgumentException("userId must be provided", nameof(userId));
            
            var envelopeCategoryOwnerId = await _envelopeRepository.GetEnvelopeCategoryOwner(envelopeCategoryId, conn);

            if (envelopeCategoryOwnerId != userId)
            {
                throw new ForbiddenException($"User {userId} does not have access to envelope category {envelopeCategoryId}");
            }
        }
    }
}
