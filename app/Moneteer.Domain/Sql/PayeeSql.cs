namespace Moneteer.Domain.Sql
{
    public static class PayeeSql
    {
        public static string Create = @"INSERT INTO app.payee (id, name, budget_id) VALUES (@Id, @Name, @BudgetId);";
        public static string Delete = @"DELETE FROM app.payee WHERE id = @Id";
        public static string GetForBudget = @"SELECT id as Id, name as Name, budget_id as BudgetId FROM app.payee WHERE budget_id = @BudgetId";
        public static string Get = @"SELECT id as Id, name as Name, budget_id as BudgetId FROM app.payee WHERE id = @Id;";
        public static string Update = @"UPDATE app.payee SET name = @Name WHERE id = @Id";
        public static string GetOwner = @"SELECT b.user_id FROM app.payee p INNER JOIN app.budget b ON p.budget_id = b.id WHERE p.id = @PayeeId";
    }
}
