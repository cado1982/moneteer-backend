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
                account
            WHERE
                budget_id = @BudgetId";

        public static string GetOwner = @"
            SELECT
                b.user_id
            FROM
                account a
            INNER JOIN 
                budget b ON a.budget_id = b.id
            WHERE 
                a.id = @AccountId";

        public static string Get = @"
            SELECT
                id as Id,
                name as Name,
                is_budget as IsBudget,
                budget_id as BudgetId
            FROM
                account
            WHERE
                id = @AccountId";

        public static string Create = @"
            INSERT INTO 
                account (id, name, budget_id, is_budget)
            VALUES (@Id, @Name, @BudgetId, @IsBudget);";

        public static string Delete = @"DELETE FROM account WHERE id = @AccountId";

        public static string Update = @"UPDATE account SET name = @Name, is_budget = @IsBudget WHERE id = @AccountId";

        public static string GetAccountBalances = @"
            WITH transactions AS (
                SELECT * FROM transaction INNER JOIN account ON transaction.account_id = account.id WHERE account.budget_id = @BudgetId
            )

            SELECT 
                a.id as AccountId,
                (SELECT COALESCE(SUM(inflow) - SUM(outflow), 0) as ClearedBalance FROM transactions t WHERE t.is_cleared = true AND t.account_id = a.id),
                (SELECT COALESCE(SUM(inflow) - SUM(outflow), 0) as UnclearedBalance FROM transactions t WHERE t.is_cleared = false AND t.account_id = a.id)
            FROM
                public.account a";

        public static string GetAccountBalance = @"
            WITH transactions AS (
                SELECT * FROM transaction WHERE account_id = @AccountId
            )

            SELECT 
                (SELECT COALESCE(SUM(inflow) - SUM(outflow), 0) as ClearedBalance FROM transactions t WHERE t.is_cleared = true),
                (SELECT COALESCE(SUM(inflow) - SUM(outflow), 0) as UnclearedBalance FROM transactions t WHERE t.is_cleared = false)";
    }
}
