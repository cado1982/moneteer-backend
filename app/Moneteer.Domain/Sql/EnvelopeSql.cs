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
                    assigned) 
                VALUES (
                    @Id,
                    @EnvelopeCategoryId,
                    @Name,
                    false,
                    false,
                    0);";

        public static string GetEnvelopesForBudget = @"
            WITH transaction_assignments AS (
	            SELECT
		            envelope_id,
		            date,
		            ta.outflow
	            FROM
		            transaction_assignment ta
	            INNER JOIN
		            transaction t ON t.id = ta.transaction_id
	            INNER JOIN
		            account a ON a.id = t.account_id
	            INNER JOIN
		            budget b ON b.id = a.budget_id
	            WHERE
		            b.id = @BudgetId AND
		            ta.envelope_id IS NOT NULL
            ),
            spending AS (
	            SELECT
		            envelope_id as EnvelopeId,
		            SUM(outflow) as SpendingLast30Days
	            FROM
		            transaction_assignments
	            WHERE
		            date >= current_date - interval '30' day
	            GROUP BY 
		            envelope_id
            ),
            average_spend AS (
	            SELECT
		            envelope_id as EnvelopeId,
		            ROUND(AVG(outflow), 2) as AverageSpend
	            FROM
		            transaction_assignments
	            GROUP BY
		            envelope_id
            )
	
            SELECT 
 	            e.id,
	            e.name,
	            e.envelope_category_id,
	            e.is_deleted,
	            e.is_hidden,
	            e.assigned,
	            COALESCE((SELECT SpendingLast30Days FROM spending WHERE EnvelopeId = e.id), 0) as SpendingLast30Days,
	            COALESCE((SELECT AverageSpend FROM average_spend WHERE EnvelopeId = e.id), 0) as AverageSpend,
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

        public static string AdjustAssigned = @"
            UPDATE
                envelope e
            SET
                assigned = assigned + @Adjustment
            WHERE
                e.id = @EnvelopeId;";

        public static string GetEnvelopeCategoriesForBudget = @"
            SELECT
	            id,
	            name
            FROM
	            envelope_category
            WHERE
	            budget_id = @BudgetId AND
	            is_deleted = false";

        public static string GetEnvelopeOwner = @"
            SELECT 
                b.user_id
            FROM
                envelope e
            INNER JOIN
                envelope_category ec ON ec.id = e.envelope_category_id
            INNER JOIN
                budget b ON b.id = ec.budget_id
            WHERE
                e.id = @EnvelopeId";

        public static string GetEnvelopeCategoryOwner = @"
            SELECT 
                b.user_id
            FROM
                envelope_category ec
            INNER JOIN
                budget b ON b.id = ec.budget_id
            WHERE
                ec.id = @EnvelopeCategoryId";

        public static string DeleteEnvelope = @"
            DELETE FROM
                envelope e
            WHERE
                e.id = @EnvelopeId";

        public static string GetEnvelopeBalances = @"
            WITH transaction_assignments AS (
                SELECT
                    e.id as envelope_id,
                    SUM(ta.outflow) as outflow
                FROM
                    transaction_assignment ta
                INNER JOIN
                    transaction t ON t.id = ta.transaction_id
                INNER JOIN
                    account a ON a.id = t.account_id
                INNER JOIN
                    envelope e ON e.id = ta.envelope_id
                WHERE
                    a.budget_id = @BudgetId
                GROUP BY
                    e.id
            )

            SELECT
                e.id as envelopeid,
                e.assigned - COALESCE(ta.outflow, 0) as balance
            FROM
                envelope e
            INNER JOIN
                envelope_category ec ON ec.id = e.envelope_category_id
            LEFT JOIN
                transaction_assignments ta ON ta.envelope_id = e.id
            WHERE
                ec.budget_id = @BudgetId";
    }
}
