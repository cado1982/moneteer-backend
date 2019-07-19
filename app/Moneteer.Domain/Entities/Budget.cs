using System;

namespace Moneteer.Domain.Entities
{
    public class Budget
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Available { get; set; }
        public string CurrencyCode { get; set; }
        public string ThousandsSeparator { get; set; }
        public string DecimalSeparator { get; set; }
        public short DecimalPlaces { get; set; }
        public SymbolLocation CurrencySymbolLocation { get; set; }
        public string DateFormat { get; set; }
        public Guid UserId { get; set; }
    }
}
