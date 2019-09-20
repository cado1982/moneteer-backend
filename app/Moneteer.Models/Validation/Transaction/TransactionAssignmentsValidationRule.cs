using System;
using System.Linq;

namespace Moneteer.Models.Validation
{
    public class TransactionAssignmentsValidationRule : ValidationRule<Transaction>
    {
        public override void Validate(Transaction model)
        {
            // Assignments must not be empty for outflow transactions
            if (model.Assignments == null || !model.Assignments.Any())
            {
                throw new ApplicationException("Assignments must be provided");
            }
            
            // Envelopes must be provided for all assignments
            if (model.Assignments.Any(a => a.Envelope == null) || model.Assignments.Any(a => a.Envelope.Id == Guid.Empty))
            {
                throw new ApplicationException("Envelope must be provided on all assignments");
            }

            // No duplicate envelopes
            if (model.Assignments.GroupBy(a => a.Envelope.Id).Any(g => g.Count() > 1))
            {
                throw new ApplicationException("No duplicate envelopes allowed.");
            }

            // Must have either outflow OR inflow but not both
            if (model.Assignments.Sum(a => a.Inflow) > 0 && model.Assignments.Sum(a => a.Outflow) > 0)
            {
                throw new ApplicationException("Assignments cannot contain outflow and inflow in the same transaction.");
            }
        }
    }
}
