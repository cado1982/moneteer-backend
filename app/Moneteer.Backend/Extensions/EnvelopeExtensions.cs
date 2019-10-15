using System;
using System.Collections.Generic;
using System.Linq;
using Entities = Moneteer.Domain.Entities;

namespace Moneteer.Backend.Extensions
{
    public static class EnvelopeExtensions
    {
        public static Models.Envelope ToModel(this Entities.Envelope entity)
        {
            if (entity == null) return null;

            return new Models.Envelope
            {
                Name = entity.Name,
                Id = entity.Id,
                EnvelopeCategory = entity.EnvelopeCategory.ToModel(),
                IsHidden = entity.IsHidden,
                Balance = 0,
                SpendingLast30Days = entity.SpendingLast30Days,
                AverageSpend = entity.AverageSpend
            };
        }

        public static Entities.Envelope ToEntity(this Models.Envelope model)
        {
            if (model == null) return null;

            return new Entities.Envelope
            {
                Name = model.Name,
                Id = model.Id,
                IsHidden = model.IsHidden,
                EnvelopeCategory =  model.EnvelopeCategory.ToEntity(),
                Assigned = 0,
                SpendingLast30Days = model.SpendingLast30Days,
                AverageSpend = model.AverageSpend
            };
        }

        public static IEnumerable<Models.Envelope> ToModels(this IEnumerable<Entities.Envelope> entities)
        {
            if (entities == null) return new List<Models.Envelope>();

            return entities.Select(t => t.ToModel());
        }

        public static IEnumerable<Entities.Envelope> ToEntities(this IEnumerable<Models.Envelope> models)
        {
            if (models == null) return new List<Entities.Envelope>();

            return models.Select(t => t.ToEntity());
        }
    }
}
