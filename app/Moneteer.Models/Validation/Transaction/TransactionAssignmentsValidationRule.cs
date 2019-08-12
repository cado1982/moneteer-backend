using System;
using System.Linq;

namespace Moneteer.Models.Validation
{
    public class TransactionAssignmentsValidationRule : ValidationRule<Transaction>
    {
        public override void Validate(Transaction model)
        {
            // Assignments must be provided but can be an empty array
            if (model.Assignments == null) 
            {
                throw new ApplicationException("Assignments must not be null");
            }

            // Assignments must not be empty for outflow transactions
            if (!model.Assignments.Any() && model.Outflow > 0)
            {
                throw new ApplicationException("Assignments must be provided for outflow transactions");
            }

            // Assignments must be empty for inflow transactions
            if (model.Assignments.Any() && model.Inflow > 0)
            {
                throw new ApplicationException("Assignments cannot be provided for inflow transactions");
            }
            
            // Assignment outflow sum must equal transaction outflow sum
            if (model.Outflow > 0 && model.Assignments.Sum(a => a.Outflow) != model.Outflow)
            {
                throw new ApplicationException("Sum of assignments outflow must match transaction outflow");
            }

            // Envelopes must be provided for all assignments
            if (model.Assignments.Any(a => a.Envelope == null) || model.Assignments.Any(a => a.Envelope.Id == Guid.Empty))
            {
                throw new ApplicationException("Envelope must be provided on all assignments");
            }
        }
    }
}
