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
            var envelopeCategoryOwnerId = await _envelopeRepository.GetEnvelopeCategoryOwner(envelopeCategoryId, conn);

            if (envelopeCategoryOwnerId != userId)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
