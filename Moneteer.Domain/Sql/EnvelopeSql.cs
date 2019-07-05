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
            WITH spending AS (SELECT
	            ta.envelope_id as EnvelopeId,
	            SUM(ta.outflow) as SpendingLast30Days
            FROM
	            transaction_assignment ta
            INNER JOIN
	            transaction t ON t.id = ta.transaction_id
            INNER JOIN
	            account a ON a.id = t.account_id
            INNER JOIN
	            budget b ON b.id = a.budget_id
            WHERE
	            b.id = '1fd69410-5947-44d0-9cf1-377be8952a84' AND
	            t.date >= current_date - interval '30' day AND
	            ta.envelope_id IS NOT NULL
            GROUP BY 
 	            ta.envelope_id)
	
            SELECT 
 	            e.id,
	            e.name,
	            e.envelope_category_id,
	            e.is_deleted,
	            e.is_hidden,
	            e.balance,
	            COALESCE((SELECT SpendingLast30Days FROM spending WHERE EnvelopeId = e.id), 0) as SpendingLast30Days,
	            ec.id,
	            ec.name,
	            ec.budget_id,
	            ec.is_deleted,
	            ec.is_hidden
            FROM 
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
