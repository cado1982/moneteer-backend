using System.Collections.Generic;

namespace Moneteer.Models
{
    public class AssignIncomeRequest
    {
        public List<AssignIncome> Assignments { get; set; }

        public AssignIncomeRequest()
        {
            Assignments = new List<AssignIncome>();
        }
    }

    public class AssignIncome
    {
        public Envelope Envelope { get; set; }
        public decimal Amount { get; set; }
    }
}
