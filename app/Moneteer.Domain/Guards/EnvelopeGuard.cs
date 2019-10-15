using Moneteer.Domain.Exceptions;
using Moneteer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Moneteer.Domain.Guards
{
    public class EnvelopeGuard
    {
        private readonly IEnvelopeRepository _envelopeRepository;

        public EnvelopeGuard(IEnvelopeRepository envelopeRepository)
        {
            _envelopeRepository = envelopeRepository;
        }

        public async Task Guard(Guid envelopeId, Guid userId, IDbConnection conn)
        {
            if (envelopeId == Guid.Empty) throw new ArgumentException("envelopeId must be provided", nameof(envelopeId));
            if (userId == Guid.Empty) throw new ArgumentException("userId must be provided", nameof(userId));

            var envelopeOwnerId = await _envelopeRepository.GetEnvelopeOwner(envelopeId, conn);

            if (envelopeOwnerId != userId)
            {
                throw new ForbiddenException($"User {userId} does not have access to envelope {envelopeId}");
            }
        }
    }
}
