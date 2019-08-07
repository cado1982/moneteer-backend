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
                transaction as t
            INNER JOIN
                account as a ON a.id = t.account_id
            LEFT JOIN
                payee as p ON p.id = t.payee_id
            LEFT JOIN
                transaction_assignment as ta ON ta.transaction_id = t.id
            LEFT JOIN
                envelope as e ON ta.envelope_id = e.id
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
                transaction as t
            INNER JOIN
                account as a ON a.id = t.account_id
            LEFT JOIN
                payee as p ON p.id = t.payee_id
            INNER JOIN
                transaction_assignment as ta ON ta.transaction_id = t.id
            LEFT JOIN
                envelope as e ON ta.envelope_id = e.id
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
                transaction as t
            INNER JOIN
                account as a ON a.id = t.account_id
            LEFT JOIN
                payee as p ON p.id = t.payee_id
            INNER JOIN
                transaction_assignment as ta ON ta.transaction_id = t.id
            LEFT JOIN
                envelope as e ON ta.envelope_id = e.id
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
                transaction as t
            INNER JOIN
                account as a ON a.id = t.account_id
            LEFT JOIN
                payee as p ON p.id = t.payee_id
            LEFT JOIN
                transaction_assignment as ta ON ta.transaction_id = t.id
            LEFT JOIN
                envelope as e ON ta.envelope_id = e.id
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
                transaction as t
            INNER JOIN
                account as a ON a.id = t.account_id
            LEFT JOIN
                payee as p ON p.id = t.payee_id
            LEFT JOIN
                transaction_assignment as ta ON ta.transaction_id = t.id
            LEFT JOIN
                envelope as e ON ta.envelope_id = e.id
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
                transaction as t
            INNER JOIN
                account as a ON a.id = t.account_id
            LEFT JOIN
                payee as p ON p.id = t.payee_id
            LEFT JOIN
                transaction_assignment as ta ON ta.transaction_id = t.id
            LEFT JOIN
                envelope as e ON ta.envelope_id = e.id
            WHERE
                a.budget_id = @BudgetId AND 
                a.is_budget IS TRUE AND
                EXTRACT(YEAR from t.date) < @Year OR (EXTRACT(YEAR from t.date) = @Year AND EXTRACT(MONTH from t.date) <= @Month)";

        public static string Create = @"
            INSERT INTO 
                transaction (
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
                transaction
            WHERE
                id = ANY(@TransactionIds)";

        public static string Update = @"
            UPDATE
                transaction
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
                transaction t
            INNER JOIN
                account a ON t.account_id = a.id
            INNER JOIN
                budget b ON a.budget_id = b.id
            WHERE 
                t.id = @TransactionId";

        public static string GetOwners = @"
            SELECT 
                b.user_id
            FROM
                transaction t
            INNER JOIN
                account a ON t.account_id = a.id
            INNER JOIN
                budget b ON a.budget_id = b.id
            WHERE 
                t.id = ANY(@TransactionIds)";

        public static string SetIsCleared = @"UPDATE transaction SET is_cleared = @IsCleared WHERE id = @TransactionId";
    }
}
