using System;
using System.Linq;

namespace Moneteer.Models.Validation
{
    public class TransactionAssignmentsValidationRule : ValidationRule<Transaction>
    {
        public override void Validate(Transaction model)
        {
            // Assignments must be provided for outflow transactions but are not required for inflow
            if ((model.Assignments == null || !model.Assignments.Any()) && model.Outflow > 0)
            {
                throw new ApplicationException("Assignments must be provided");
            }
            
            // Assignment outflow sum must equal transaction outflow sum
            if (model.Assignments != null &&model.Outflow > 0 && model.Assignments.Sum(a => a.Outflow) != model.Outflow)
            {
                throw new ApplicationException("Sum of assignments outflow must match transaction outflow");
            }

            // Envelopes must be provided for all assignments
            if (model.Assignments != null && model.Assignments.Any(a => a.Envelope == null) || model.Assignments.Any(a => a.Envelope.Id == Guid.Empty))
            {
                throw new ApplicationException("Envelope must be provided on all assignments");
            }
        }
    }
}
