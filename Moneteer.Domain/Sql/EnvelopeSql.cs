namespace Moneteer.Domain.Sql
{
    public static class EnvelopeSql
    {
        public static string CreateEnvelopeCategory = @"
            INSERT INTO envelope_category (id, budget_id, name, is_deleted, is_hidden) VALUES (@Id, @BudgetId, @Name, false, false);";

        public static string CreateEnvelope = @"
            INSERT INTO 
                envelope (
                    id,
                    envelope_category_id,
                    name,
                    is_deleted,
                    is_hidden,
                    balance) 
                VALUES (
                    @Id,
                    @EnvelopeCategoryId,
                    @Name,
                    false,
                    false,
                    0);";

        public static string GetForBudget = @"
            SELECT * FROM 
                envelope e 
            INNER JOIN 
                envelope_category ec ON e.envelope_category_id = ec.id 
            WHERE 
                ec.budget_id = @BudgetId AND                
                e.is_deleted = false AND 
                ec.is_deleted = false";

        public static string AdjustBalance = @"
            UPDATE
                envelope e
            SET
                balance = balance + @Adjustment
            WHERE
                e.id = @EnvelopeId;";
    }
}
