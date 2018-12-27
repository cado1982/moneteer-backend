using System;
using Entities = Moneteer.Domain.Entities;

namespace Moneteer.Backend.Extensions
{
    public static class AccountExtensions
    {
        public static Entities.Account ToEntity(this Models.Account model)
        {
            if (model == null) return null;

            return new Entities.Account
            {
                Id = model.Id,
                Name = model.Name,
                BudgetId = model.BudgetId,
                IsBudget = model.IsBudget
            };
        }

        public static Models.Account ToModel(this Entities.Account entity)
        {
            if (entity == null) return null;

            return new Models.Account
            {
                Id = entity.Id,
                Name = entity.Name,
                IsBudget = entity.IsBudget,
                BudgetId = entity.BudgetId
            };
        }
    }
}
