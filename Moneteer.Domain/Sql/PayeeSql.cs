namespace Moneteer.Domain.Sql
{
    public static class PayeeSql
    {
        public static string Create = @"INSERT INTO payee (id, name, budget_id) VALUES (@Id, @Name, @BudgetId);";
        public static string Delete = @"DELETE FROM payee WHERE id = @Id";
        public static string GetForBudget = @"SELECT id as Id, name as Name, budget_id as BudgetId FROM payee WHERE budget_id = @BudgetId";
        public static string Get = @"SELECT id as Id, name as Name, budget_id as BudgetId FROM payee WHERE id = @Id;";
        public static string Update = @"UPDATE payee SET name = @Name WHERE id = @Id";
        public static string GetOwner = @"SELECT b.user_id FROM payee p INNER JOIN budget b ON p.budget_id = b.id WHERE p.id = @PayeeId";
    }
}
