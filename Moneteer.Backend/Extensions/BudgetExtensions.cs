using System;
using Entities = Moneteer.Domain.Entities;

namespace Moneteer.Backend.Extensions
{
    public static class BudgetExtensions
    {
        public static Entities.Budget ToEntity(this Models.Budget model, Guid userId)
        {
            if (model == null) return null;

            return new Entities.Budget
            {
                Id = model.Id,
                Name = model.Name,
                UserId = userId,
                Available = model.Available,
                CurrencyCode = model.Currency.Code,
                CurrencySymbolLocation = model.CurrencySymbolLocation.ToEntity(),
                DecimalPlaces = model.CurrencyFormat.DecimalPlaces,
                DecimalSeparator = model.CurrencyFormat.DecimalSeparator,
                ThousandsSeparator = model.CurrencyFormat.ThousandsSeparator,
                DateFormat = model.DateFormat
            };
        }

        public static Models.Budget ToModel(this Entities.Budget entity)
        {
            if (entity == null) return null;

            return new Models.Budget
            {
                Id = entity.Id,
                Name = entity.Name,
                Currency = new Models.Currency
                {
                    Name = String.Empty, // Backend doesn't care about the name
                    Symbol = String.Empty, // or the symbol
                    Code = entity.CurrencyCode,
                },
                Available = entity.Available,
                CurrencySymbolLocation = entity.CurrencySymbolLocation.ToModel(),
                CurrencyFormat = new Models.CurrencyFormat
                {
                    DecimalPlaces = entity.DecimalPlaces,
                    DecimalSeparator = entity.DecimalSeparator,
                    ThousandsSeparator = entity.ThousandsSeparator
                },
                DateFormat = entity.DateFormat
            };
        }

        public static Models.SymbolLocation ToModel(this Entities.SymbolLocation entity)
        {
            return (Models.SymbolLocation)Enum.Parse(typeof(Models.SymbolLocation), entity.ToString());
        }

        public static Entities.SymbolLocation ToEntity(this Models.SymbolLocation model)
        {
            return (Entities.SymbolLocation)Enum.Parse(typeof(Entities.SymbolLocation), model.ToString());
        }
    }
}
