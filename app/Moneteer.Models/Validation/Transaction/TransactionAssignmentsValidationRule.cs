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
            
            // Assignment outflow sum must equal transaction outflow sum
            if (model.Outflow > 0 && model.Assignments.Sum(a => a.Outflow) != model.Outflow)
            {
                throw new ApplicationException("Sum of assignments outflow must match transaction outflow");
            }

            // Assignment outflow sum must equal transaction outflow sum
            if (model.Inflow > 0 && model.Assignments.Sum(a => a.Inflow) != model.Inflow)
            {
                throw new ApplicationException("Sum of assignments inflow must match transaction inflow");
            }

            // Envelopes must be provided for all assignments
            if (model.Assignments.Any(a => a.Envelope == null) || model.Assignments.Any(a => a.Envelope.Id == Guid.Empty))
            {
                throw new ApplicationException("Envelope must be provided on all assignments");
            }
        }
    }
}
