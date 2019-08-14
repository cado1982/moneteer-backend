namespace Moneteer.Domain.Sql
{
    public static class TransactionSql
    {
        public static string GetForBudget = @"
            SELECT 
	            t.id as Id, 
                t.account_id as AccountId,
                t.payee_id as PayeeId,
                t.is_cleared as IsCleared,
                t.date as Date,
                t.description as Description,
                t.is_reconciled as IsReconciled,
                t.inflow as Inflow,
                t.outflow as Outflow,
                a.id as Id,
                a.name as Name,
                a.is_budget as IsBudget,
                a.budget_id as BudgetId,
                p.id as Id,
                p.name as Name,
                p.last_envelope_id as LastEnvelopeId,
                ta.id as Id,
                ta.inflow as Inflow,
                ta.outflow as Outflow,
                ta.envelope_id as EnvelopeId,
                e.id as Id,
                e.name as Name
            FROM
                app.transaction as t
            INNER JOIN
                app.account as a ON a.id = t.account_id
            LEFT JOIN
                app.payee as p ON p.id = t.payee_id
            LEFT JOIN
                app.transaction_assignment as ta ON ta.transaction_id = t.id
            LEFT JOIN
                app.envelope as e ON ta.envelope_id = e.id
            WHERE
                a.budget_id = @BudgetId;";

        public static string GetForAccount = @"
            SELECT 
	            t.id as Id, 
                t.account_id as AccountId,
                t.payee_id as PayeeId,
                t.is_cleared as IsCleared,
                t.date as Date,
                t.description as Description,
                t.is_reconciled as IsReconciled,
                t.inflow as Inflow,
                t.outflow as Outflow,
                a.id as Id,
                a.name as Name,
                a.is_budget as IsBudget,
                a.budget_id as BudgetId,
                p.id as Id,
                p.name as Name,
                p.last_envelope_id as LastEnvelopeId,
                ta.id as Id,
                ta.inflow as Inflow,
                ta.outflow as Outflow,
                ta.envelope_id as EnvelopeId,
                e.id as Id,
                e.name as Name
            FROM
                app.transaction as t
            INNER JOIN
                app.account as a ON a.id = t.account_id
            LEFT JOIN
                app.payee as p ON p.id = t.payee_id
            INNER JOIN
                app.transaction_assignment as ta ON ta.transaction_id = t.id
            LEFT JOIN
                app.envelope as e ON ta.envelope_id = e.id
            WHERE
                a.id = @AccountId;";

        public static string GetForMonth = @"
            SELECT 
	            t.id as Id, 
                t.account_id as AccountId,
                t.payee_id as PayeeId,
                t.is_cleared as IsCleared,
                t.date as Date,
                t.description as Description,
                t.is_reconciled as IsReconciled,
                t.inflow as Inflow,
                t.outflow as Outflow,
                a.id as Id,
                a.name as Name,
                a.is_budget as IsBudget,
                a.budget_id as BudgetId,
                p.id as Id,
                p.name as Name,
                p.last_envelope_id as LastEnvelopeId,
                ta.id as Id,
                ta.inflow as Inflow,
                ta.outflow as Outflow,
                ta.envelope_id as EnvelopeId,
                e.id as Id,
                e.name as Name
            FROM
                app.transaction as t
            INNER JOIN
                app.account as a ON a.id = t.account_id
            LEFT JOIN
                app.payee as p ON p.id = t.payee_id
            INNER JOIN
                app.transaction_assignment as ta ON ta.transaction_id = t.id
            LEFT JOIN
                app.envelope as e ON ta.envelope_id = e.id
            WHERE
                -- We want to take all transactions with a date of this month along with all
                -- transactions from last month that were marked as income for this month
                a.budget_id = @BudgetId AND 
                ((EXTRACT(YEAR from t.date) = @Year AND EXTRACT(MONTH from t.date) = @Month) OR 
                 (EXTRACT(YEAR from t.date) = EXTRACT(YEAR from make_date(@Year, @Month, 1) - INTERVAL '1 month') AND 
                  EXTRACT(MONTH from t.date) = EXTRACT(MONTH from make_date(@Year, @Month, 1) - INTERVAL '1 month') AND
                  cc.name = 'Available next month')
                )";

        public static string GetByIds = @"
            SELECT 
	            t.id as Id, 
                t.account_id as AccountId,
                t.payee_id as PayeeId,
                t.is_cleared as IsCleared,
                t.date as Date,
                t.description as Description,
                t.is_reconciled as IsReconciled,
                t.inflow as Inflow,
                t.outflow as Outflow,
                a.id as Id,
                a.name as Name,
                a.is_budget as IsBudget,
                a.budget_id as BudgetId,
                p.id as Id,
                p.name as Name,
                p.last_envelope_id as LastEnvelopeId,
                ta.id as Id,
                ta.inflow as Inflow,
                ta.outflow as Outflow,
                ta.envelope_id as EnvelopeId,
                e.id as Id,
                e.name as Name
            FROM
                app.transaction as t
            INNER JOIN
                app.account as a ON a.id = t.account_id
            LEFT JOIN
                app.payee as p ON p.id = t.payee_id
            LEFT JOIN
                app.transaction_assignment as ta ON ta.transaction_id = t.id
            LEFT JOIN
                app.envelope as e ON ta.envelope_id = e.id
            WHERE
                t.id = ANY(@Ids)";

        public static string GetOnOrBefore = @"
            SELECT 
	            t.id as Id, 
                t.account_id as AccountId,
                t.payee_id as PayeeId,
                t.is_cleared as IsCleared,
                t.date as Date,
                t.description as Description,
                t.is_reconciled as IsReconciled,
                t.inflow as Inflow,
                t.outflow as Outflow,
                a.id as Id,
                a.name as Name,
                a.is_budget as IsBudget,
                a.budget_id as BudgetId,
                p.id as Id,
                p.name as Name,
                p.last_envelope_id as LastEnvelopeId,
                ta.id as Id,
                ta.inflow as Inflow,
                ta.outflow as Outflow,
                ta.envelope_id as EnvelopeId,
                e.id as Id,
                e.name as Name
            FROM
                app.transaction as t
            INNER JOIN
                app.account as a ON a.id = t.account_id
            LEFT JOIN
                app.payee as p ON p.id = t.payee_id
            LEFT JOIN
                app.transaction_assignment as ta ON ta.transaction_id = t.id
            LEFT JOIN
                app.envelope as e ON ta.envelope_id = e.id
            WHERE
                a.budget_id = @BudgetId AND 
                EXTRACT(YEAR from t.date) < @Year OR (EXTRACT(YEAR from t.date) = @Year AND EXTRACT(MONTH from t.date) <= @Month)";

        public static string GetBudgetOnOrBefore = @"
            SELECT 
	            t.id as Id, 
                t.account_id as AccountId,
                t.payee_id as PayeeId,
                t.is_cleared as IsCleared,
                t.date as Date,
                t.description as Description,
                t.is_reconciled as IsReconciled,
                t.inflow as Inflow,
                t.outflow as Outflow,
                a.id as Id,
                a.name as Name,
                a.is_budget as IsBudget,
                a.budget_id as BudgetId,
                p.id as Id,
                p.name as Name,
                p.last_envelope_id as LastEnvelopeId,
                ta.id as Id,
                ta.inflow as Inflow,
                ta.outflow as Outflow,
                ta.envelope_id as EnvelopeId,
                e.id as Id,
                e.name as Name
            FROM
                app.transaction as t
            INNER JOIN
                app.account as a ON a.id = t.account_id
            LEFT JOIN
                app.payee as p ON p.id = t.payee_id
            LEFT JOIN
                app.transaction_assignment as ta ON ta.transaction_id = t.id
            LEFT JOIN
                app.envelope as e ON ta.envelope_id = e.id
            WHERE
                a.budget_id = @BudgetId AND 
                a.is_budget IS TRUE AND
                EXTRACT(YEAR from t.date) < @Year OR (EXTRACT(YEAR from t.date) = @Year AND EXTRACT(MONTH from t.date) <= @Month)";

        public static string Create = @"
            INSERT INTO 
                app.transaction (
                    id,
                    account_id,
                    payee_id,
                    is_cleared,
                    date,
                    description,
                    is_reconciled,
                    inflow,
                    outflow) 
                VALUES (
                    @Id,
                    @AccountId,
                    @PayeeId,
                    @IsCleared,
                    @Date,
                    @Description,
                    @IsReconciled, 
                    @Inflow,
                    @Outflow);";

        public static string Delete = @"
            DELETE FROM
                app.transaction
            WHERE
                id = ANY(@TransactionIds)";

        public static string Update = @"
            UPDATE
                app.transaction
            SET
                account_id = @AccountId,
                payee_id = @PayeeId,
                is_cleared = @IsCleared,
                date = @Date,
                description = @Description,
                is_reconciled = @IsReconciled,
                inflow = @Inflow,
                outflow = @Outflow
            WHERE
                id = @Id";

        public static string GetOwner = @"
            SELECT 
                b.user_id
            FROM
                app.transaction t
            INNER JOIN
                app.account a ON t.account_id = a.id
            INNER JOIN
                app.budget b ON a.budget_id = b.id
            WHERE 
                t.id = @TransactionId";

        public static string GetOwners = @"
            SELECT 
                b.user_id
            FROM
                app.transaction t
            INNER JOIN
                app.account a ON t.account_id = a.id
            INNER JOIN
                app.budget b ON a.budget_id = b.id
            WHERE 
                t.id = ANY(@TransactionIds)";

        public static string SetIsCleared = @"UPDATE app.transaction SET is_cleared = @IsCleared WHERE id = @TransactionId";

        public static string GetRecentTransactionsByEnvelope = @"
            WITH transactions AS (
                SELECT
                    ROW_NUMBER() OVER (PARTITION BY envelope_id ORDER BY date) AS r,
                    e.id as envelope_id,
                    t.date,
                    SUM(ta.outflow) as outflow,
                    p.name as payee,
                    ec.budget_id as budget_id
                FROM
                    app.transaction_assignment ta
                INNER JOIN
                    app.envelope e ON e.id = ta.envelope_id
                INNER JOIN
                    app.envelope_category ec ON ec.id = e.envelope_category_id
                INNER JOIN
                    app.transaction t ON t.id = ta.transaction_id
                LEFT JOIN
                    app.payee p ON p.id = t.payee_id
                GROUP BY	
                    t.id, t.date, e.id, p.name, ec.budget_id, ta.envelope_id
            )

            SELECT 
                t.envelope_id as EnvelopeId,
                t.date as Date,
                t.outflow as Amount,
                t.payee as Payee
            FROM 
                transactions t
            WHERE
                t.r <= @NumberOfTransactions AND
                budget_id = @BudgetId";
    }
}
