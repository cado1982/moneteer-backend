using System;
using System.Collections.Generic;
using System.Linq;
using Entities = Moneteer.Domain.Entities;

namespace Moneteer.Backend.Extensions
{
    public static class TransactionAssignmentExtensions
    {
        public static Models.TransactionAssignment ToModel(this Entities.TransactionAssignment entity)
        {
            if (entity == null) return null;

            return new Models.TransactionAssignment
            {
                Id = entity.Id,
                Envelope = entity.Envelope.ToModel(),
                Inflow = entity.Inflow,
                Outflow = entity.Outflow
            };
        }

        public static Entities.TransactionAssignment ToEntity(this Models.TransactionAssignment model, Guid? transactionId = null)
        {
            if (model == null) return null;

            return new Entities.TransactionAssignment
            {
                Id = model.Id,
                Envelope = model.Envelope.ToEntity(),
                Inflow = model.Inflow,
                Outflow = model.Outflow
            };
        }

        public static IEnumerable<Models.TransactionAssignment> ToModels(this IEnumerable<Entities.TransactionAssignment> entities)
        {
            if (entities == null) return null;

            return entities.Select(t => t.ToModel());
        }

        public static IEnumerable<Entities.TransactionAssignment> ToEntities(this IEnumerable<Models.TransactionAssignment> models)
        {
            if (models == null) return null;

            return models.Select(t => t.ToEntity());
        }
    }
}
