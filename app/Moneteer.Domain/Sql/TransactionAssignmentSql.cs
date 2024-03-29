﻿namespace Moneteer.Domain.Sql
{
    public static class TransactionAssignmentSql
    {
        public static string Create = @"INSERT INTO app.transaction_assignment(id, transaction_id, envelope_id, inflow, outflow) VALUES(@Id, @TransactionId, @EnvelopeId, @Inflow, @Outflow)";
        public static string DeleteByIds = @"DELETE FROM app.transaction_assignment WHERE id = @AssignmentId";
        public static string DeleteByTransactionId = @"DELETE FROM app.transaction_assignment WHERE transaction_id = @TransactionId";
    }
}
