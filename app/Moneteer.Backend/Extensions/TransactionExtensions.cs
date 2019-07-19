using System;
using System.Collections.Generic;
using System.Linq;
using Entities = Moneteer.Domain.Entities;

namespace Moneteer.Backend.Extensions
{

    public static class TransactionExtensions
    {
        public static Models.Transaction ToModel(this Entities.Transaction entity)
        {
            if (entity == null) return null;

            return new Models.Transaction
            {
                Account = entity.Account.ToModel(),
                Date = entity.Date,
                Description = entity.Description,
                Id = entity.Id,
                Inflow = entity.Inflow,
                Outflow = entity.Outflow,
                IsCleared = entity.IsCleared,
                IsReconciled = entity.IsReconciled,
                Payee = entity.Payee.ToModel(),
                Assignments = entity.Assignments.ToModels().ToList()
            };
        }

        public static Entities.Transaction ToEntity(this Models.Transaction model)
        {
            if (model == null) return null;

            return new Entities.Transaction
            {
                Account = model.Account.ToEntity(),
                Date = model.Date,
                Id = model.Id,
                Description = model.Description,
                Inflow = model.Inflow,
                IsCleared = model.IsCleared,
                IsReconciled = model.IsReconciled,
                Outflow = model.Outflow,
                Payee = model.Payee.ToEntity(),
                Assignments = model.Assignments.ToEntities()?.ToList()
            };
        }

        public static IEnumerable<Models.Transaction> ToModels(this IEnumerable<Entities.Transaction> entities)
        {
            if (entities == null) return null;

            return entities.Select(t => t.ToModel());
        }

        public static IEnumerable<Entities.Transaction> ToEntities(this IEnumerable<Models.Transaction> models)
        {
            if (models == null) return null;

            return models.Select(t => t.ToEntity());
        }
    }
}
