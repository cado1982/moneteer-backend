using Moneteer.Domain.Entities;
using System.Collections.Generic;

namespace Moneteer.Domain.Entities.Comparers
{
    public class EnvelopeCategoryEqualityComparer : IEqualityComparer<EnvelopeCategory>
    {
        public bool Equals(EnvelopeCategory x, EnvelopeCategory y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(EnvelopeCategory obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
