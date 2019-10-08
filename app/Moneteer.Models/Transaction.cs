using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Moneteer.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public Account Account { get; set; }

        public bool IsCleared { get; set; }

        public bool IsReconciled { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public List<TransactionAssignment> Assignments { get; set; }
    }
}
