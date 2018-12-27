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
    }
}
