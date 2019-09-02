using System;

namespace Moneteer.Domain.Entities
{
    public class SubscriptionStatus
    {
        public DateTime? SubscriptionExpiry { get; set; }
        public DateTime TrialExpiry { get; set; }
    }
}