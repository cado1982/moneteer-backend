using System.Collections.Generic;

namespace Moneteer.Domain.Entities.Comparers
{
    public class TransactionAssignmentEqualityComparer : IEqualityComparer<TransactionAssignment>
    {
        public bool Equals(TransactionAssignment x, TransactionAssignment y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(TransactionAssignment obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
