namespace Moneteer.Domain.Sql
{
    public static class BudgetSql
    {
        public static string Create = @"
            INSERT INTO 
                app.budget (id, 
                        name, 
                        user_id,
                        currency_code,
                        thousands_separator,
                        decimal_separator,
                        decimal_places,
                        currency_symbol_location,
                        date_format)
            VALUES 
                (@Id, 
                 @Name, 
                 @UserId,
                 @CurrencyCode, 
                 @ThousandsSeparator, 
                 @DecimalSeparator, 
                 @DecimalPlaces, 
                 @CurrencySymbolLocation, 
                 @DateFormat);";

        public static string GetOwner = @"SELECT user_id FROM app.budget WHERE id = @BudgetId";

        public static string Get = @"
            SELECT 
                id as Id,
                name as Name,
                thousands_separator as ThousandsSeparator,
                decimal_separator as DecimalSeparator,
                decimal_places as DecimalPlaces,
                currency_code as CurrencyCode,
                currency_symbol_location as CurrencySymbolLocation,
                date_format as DateFormat
            FROM 
                app.budget 
            WHERE id = @id";
        
        public static string Delete = @"DELETE FROM app.budget WHERE id = @BudgetId";

        public static string GetAllForUser = @"
            SELECT 
                id as Id,
                name as Name,
                thousands_separator as ThousandsSeparator,
                decimal_separator as DecimalSeparator,
                decimal_places as DecimalPlaces,
                currency_code as CurrencyCode,
                currency_symbol_location as CurrencySymbolLocation,
                date_format as DateFormat
            FROM
                app.budget
            WHERE user_id = @UserId";
    }
}
