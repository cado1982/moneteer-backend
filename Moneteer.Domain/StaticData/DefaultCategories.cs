using Moneteer.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Moneteer.Domain.StaticData
{
    public static class DefaultCategories
    {
        public static BudgetEnvelopes GenerateDefaultCategories(Guid budgetId)
        {
            var result = new BudgetEnvelopes
            {
                Envelopes = new List<Envelope>(),
                Categories = new List<EnvelopeCategory>()
            };

            var monthlyBills = new EnvelopeCategory { Id = Guid.NewGuid(), BudgetId = budgetId, Name = "Monthly Bills" };
            var everydayExpenses = new EnvelopeCategory { Id = Guid.NewGuid(), BudgetId = budgetId, Name = "Everyday Expenses" };
            var rainyDayFunds = new EnvelopeCategory { Id = Guid.NewGuid(), BudgetId = budgetId, Name = "Rainy Day Funds" };
            var savingsGoals = new EnvelopeCategory { Id = Guid.NewGuid(), BudgetId = budgetId, Name = "Savings Goals" };
            var debt = new EnvelopeCategory { Id = Guid.NewGuid(), BudgetId = budgetId, Name = "Debt" };
            var giving = new EnvelopeCategory { Id = Guid.NewGuid(), BudgetId = budgetId, Name = "Giving" };

            result.Categories.Add(monthlyBills);
            result.Categories.Add(everydayExpenses);
            result.Categories.Add(rainyDayFunds);
            result.Categories.Add(savingsGoals);
            result.Categories.Add(debt);
            result.Categories.Add(giving);

            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = monthlyBills, Name = "Rent/Mortgage" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = monthlyBills, Name = "Phone" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = monthlyBills, Name = "Internet" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = monthlyBills, Name = "Cable TV" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = monthlyBills, Name = "Electricity" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = monthlyBills, Name = "Water" });

            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = everydayExpenses, Name = "Spending Money"});
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = everydayExpenses, Name = "Groceries" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = everydayExpenses, Name = "Fuel" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = everydayExpenses, Name = "Restaurants" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = everydayExpenses, Name = "Medical/Dental" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = everydayExpenses, Name = "Clothing" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = everydayExpenses, Name = "Household Goods" });

            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = rainyDayFunds, Name = "Emergency Fund" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = rainyDayFunds, Name = "Car Maintenance" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = rainyDayFunds, Name = "Car Insurance" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = rainyDayFunds, Name = "Birthdays" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = rainyDayFunds, Name = "Christmas" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = rainyDayFunds, Name = "Renters Insurance" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = rainyDayFunds, Name = "Retirement" });

            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = savingsGoals, Name = "Car Replacement"});
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = savingsGoals, Name = "Vacation" });

            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = debt, Name = "Car Payment" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = debt, Name = "Student Loan Payment" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = debt, Name = "Pre Moneteer Debt" });

            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = giving, Name = "Tithing" });
            result.Envelopes.Add(new Envelope { Id = Guid.NewGuid(), EnvelopeCategory = giving, Name = "Charitable" });

            return result;
        }
    }
}
