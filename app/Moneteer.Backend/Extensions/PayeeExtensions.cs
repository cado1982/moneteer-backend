using System;
using System.Collections.Generic;
using System.Linq;
using Entities = Moneteer.Domain.Entities;

namespace Moneteer.Backend.Extensions
{
    public static class PayeeExtensions
    {
        public static Models.Payee ToModel(this Entities.Payee entity)
        {
            if (entity == null) return null;

            return new Models.Payee
            {
                Name = entity.Name,
                Id = entity.Id,
                LastEnvelope = entity.LastEnvelope.ToModel()
            };
        }

        public static Entities.Payee ToEntity(this Models.Payee model, Guid? budgetId = null)
        {
            if (model == null) return null;

            return new Entities.Payee
            {
                LastEnvelope = model.LastEnvelope.ToEntity(),
                Id = model.Id,
                Name = model.Name,
                BudgetId = budgetId == null ? Guid.Empty : (Guid)budgetId
            };
        }

        public static IEnumerable<Models.Payee> ToModels(this IEnumerable<Entities.Payee> entities)
        {
            return entities.Select(t => t.ToModel());
        }

        public static IEnumerable<Entities.Payee> ToEntities(this IEnumerable<Models.Payee> models)
        {
            return models.Select(t => t.ToEntity());
        }
    }
}
