namespace Moneteer.Domain.Sql
{
    public static class EnvelopeSql
    {
        public static string CreateEnvelopeCategory = @"
            INSERT INTO app.envelope_category (id, budget_id, name, is_hidden) VALUES (@Id, @BudgetId, @Name, false);";

        public static string CreateEnvelope = @"
            INSERT INTO 
                app.envelope (
                    id,
                    envelope_category_id,
                    name,
                    is_hidden,
                    assigned) 
                VALUES (
                    @Id,
                    @EnvelopeCategoryId,
                    @Name,
                    false,
                    0);";

        public static string GetEnvelopesForBudget = @"
            WITH transaction_assignments AS (
	            SELECT
		            ta.envelope_id,
		            t.date,
		            ta.outflow
	            FROM
		            app.transaction_assignment ta
	            INNER JOIN
		            app.transaction t ON t.id = ta.transaction_id
	            INNER JOIN
		            app.account a ON a.id = t.account_id
	            INNER JOIN
		            app.budget b ON b.id = a.budget_id
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
	            e.is_hidden as IsHidden,
	            e.assigned,
	            COALESCE((SELECT SpendingLast30Days FROM spending WHERE EnvelopeId = e.id), 0) as SpendingLast30Days,
	            COALESCE((SELECT AverageSpend FROM average_spend WHERE EnvelopeId = e.id), 0) as AverageSpend,
	            ec.id,
	            ec.name,
	            ec.budget_id,
	            ec.is_hidden
            FROM 
	            app.envelope e 
            INNER JOIN 
	            app.envelope_category ec ON e.envelope_category_id = ec.id 
            WHERE    
                ec.budget_id = @BudgetId";

        public static string AdjustAssigned = @"
            UPDATE
                app.envelope e
            SET
                assigned = assigned + @Adjustment
            WHERE
                e.id = @EnvelopeId;";

        public static string GetEnvelopeCategoriesForBudget = @"
            SELECT
	            id,
	            name
            FROM
	            app.envelope_category
            WHERE
	            budget_id = @BudgetId";

        public static string GetEnvelopeOwner = @"
            SELECT 
                b.user_id
            FROM
                app.envelope e
            INNER JOIN
                app.envelope_category ec ON ec.id = e.envelope_category_id
            INNER JOIN
                app.budget b ON b.id = ec.budget_id
            WHERE
                e.id = @EnvelopeId";

        public static string GetEnvelopeCategoryOwner = @"
            SELECT 
                b.user_id
            FROM
                app.envelope_category ec
            INNER JOIN
                budget b ON b.id = ec.budget_id
            WHERE
                ec.id = @EnvelopeCategoryId";

        public static string DeleteEnvelope = @"
            DELETE FROM
                app.envelope e
            WHERE
                e.id = @EnvelopeId";

        public static string GetEnvelopeBalances = @"
            WITH transaction_assignments AS (
	            SELECT
		            e.id as envelope_id,
		            SUM(ta.inflow - ta.outflow) as amount
	            FROM
		            app.transaction_assignment ta
	            INNER JOIN
		            app.transaction t ON t.id = ta.transaction_id
	            INNER JOIN
		            app.account a ON a.id = t.account_id
	            INNER JOIN
		            app.envelope e ON e.id = ta.envelope_id
                WHERE
                    a.budget_id = @BudgetId
	            GROUP BY
		            e.id
            )

            SELECT
	            e.id as envelopeid,
	            e.assigned + COALESCE(ta.amount, 0) as balance
            FROM
	            app.envelope e
            INNER JOIN
                app.envelope_category ec ON ec.id = e.envelope_category_id
            LEFT JOIN
	            transaction_assignments ta ON ta.envelope_id = e.id
            WHERE
                ec.budget_id = @BudgetId";

        public static string UpdateEnvelope = @"
            UPDATE 
                app.envelope
            SET
                name = @Name,
                envelope_category_id = @EnvelopeCategoryId,
                is_hidden = @IsHidden
            WHERE
                envelope.id = @EnvelopeId";

        public static string UpdateEnvelopeIsHidden = @"
            UPDATE 
                app.envelope
            SET
                is_hidden = @IsHidden
            WHERE
                envelope.id = @EnvelopeId";
    }
}
