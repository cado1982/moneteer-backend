using System;

namespace Moneteer.Domain.Entities
{
    public class AccountBalance
    {
        public Guid AccountId { get; set; }
        public decimal ClearedBalance { get; set; }
        public decimal UnclearedBalance { get; set; }
    }
}
