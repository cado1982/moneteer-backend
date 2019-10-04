namespace Moneteer.Domain.Sql
{
    public static class AccountSql
    {
        public static string GetAllForBudget = @"
            SELECT
                id as Id,
                name as Name,
                is_budget as IsBudget
            FROM
                app.account
            WHERE
                budget_id = @BudgetId";

        public static string GetOwner = @"
            SELECT
                b.user_id
            FROM
                app.account a
            INNER JOIN 
                app.budget b ON a.budget_id = b.id
            WHERE 
                a.id = @AccountId";

        public static string Get = @"
            SELECT
                id as Id,
                name as Name,
                is_budget as IsBudget,
                budget_id as BudgetId
            FROM
                app.account
            WHERE
                id = @AccountId";

        public static string Create = @"
            INSERT INTO 
                app.account (id, name, budget_id, is_budget)
            VALUES (@Id, @Name, @BudgetId, @IsBudget);";

        public static string Delete = @"DELETE FROM app.account WHERE id = @AccountId";

        public static string Update = @"UPDATE app.account SET name = @Name, is_budget = @IsBudget WHERE id = @AccountId";

        public static string GetAccountBalances = @"
            WITH transaction_assignments AS (
                SELECT 
                    ta.envelope_id,
                    ta.inflow,
                    ta.outflow,
                    t.account_id,
                    t.is_cleared
                FROM 
                    app.transaction_assignment ta
                INNER JOIN
                    app.transaction t ON t.id = ta.transaction_id 
                INNER JOIN 
                    app.account a ON t.account_id = a.id 
                WHERE 
                    a.budget_id = @BudgetId
            )

            SELECT 
                a.id as AccountId,
                (SELECT COALESCE(SUM(inflow) - SUM(outflow), 0) as ClearedBalance FROM transaction_assignments t WHERE t.is_cleared = true AND t.account_id = a.id),
                (SELECT COALESCE(SUM(inflow) - SUM(outflow), 0) as UnclearedBalance FROM transaction_assignments t WHERE t.is_cleared = false AND t.account_id = a.id)
            FROM
                app.account a
            WHERE
                a.budget_id = @BudgetId";

        public static string GetAccountBalance = @"
            WITH transactions AS (
                SELECT * FROM app.transaction WHERE account_id = @AccountId
            )

            SELECT 
                (SELECT COALESCE(SUM(inflow) - SUM(outflow), 0) as ClearedBalance FROM transactions t WHERE t.is_cleared = true),
                (SELECT COALESCE(SUM(inflow) - SUM(outflow), 0) as UnclearedBalance FROM transactions t WHERE t.is_cleared = false)";
    }
}
