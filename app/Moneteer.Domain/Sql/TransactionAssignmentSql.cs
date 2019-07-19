namespace Moneteer.Domain.Sql
{
    public static class TransactionAssignmentSql
    {
        public static string Create = @"INSERT INTO transaction_assignment(id, transaction_id, envelope_id, inflow, outflow) VALUES(@Id, @TransactionId, @EnvelopeId, @Inflow, @Outflow)";
    }
}
