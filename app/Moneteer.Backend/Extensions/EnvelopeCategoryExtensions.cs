using System;
using System.Collections.Generic;
using System.Linq;
using Entities = Moneteer.Domain.Entities;

namespace Moneteer.Backend.Extensions
{
    public static class EnvelopeCategoryExtensions
    {
        public static Models.EnvelopeCategory ToModel(this Entities.EnvelopeCategory entity)
        {
            if (entity == null) return null;

            return new Models.EnvelopeCategory
            {
                Name = entity.Name,
                Id = entity.Id,
                IsHidden = entity.IsHidden,
                IsToggled = entity.IsToggled
            };
        }

        public static Entities.EnvelopeCategory ToEntity(this Models.EnvelopeCategory model, Guid? budgetId = null)
        {
            if (model == null) return null;

            return new Entities.EnvelopeCategory
            {
                Name = model.Name,
                Id = model.Id,
                BudgetId = budgetId == null ? Guid.Empty : (Guid)budgetId,
                IsHidden = model.IsHidden,
                IsToggled = model.IsToggled
            };
        }

        public static IEnumerable<Models.EnvelopeCategory> ToModels(this IEnumerable<Entities.EnvelopeCategory> entities)
        {
            return entities.Select(t => t.ToModel());
        }

        public static IEnumerable<Entities.EnvelopeCategory> ToEntities(this IEnumerable<Models.EnvelopeCategory> models)
        {
            return models.Select(t => t.ToEntity());
        }
    }
}
