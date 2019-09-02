namespace Moneteer.Domain.Sql
{
    public static class SubscriptionSql
    {
        public static string GetSubscriptionStatus = @"
            SELECT 
                subscription_expiry as SubscriptionExpiry,
                trial_expiry as TrialExpiry
            FROM
                identity.users
            WHERE
                id = @UserId";
    }
}
