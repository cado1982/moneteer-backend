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
            var envelopeOwnerId = await _envelopeRepository.GetEnvelopeOwner(envelopeId, conn);

            if (envelopeOwnerId != userId)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
