namespace Moneteer.Domain
{
    public static class ClaimType
    {
        private const string _namespace = "https://www.moneteer.com/";

        public static string TrialExpiry = _namespace + "trial-expiry";
        public static string SubscriptionExpiry = _namespace + "subscription-expiry";
        public static string StripeCustomerId = _namespace + "stripe-customer-id";
    }
}
