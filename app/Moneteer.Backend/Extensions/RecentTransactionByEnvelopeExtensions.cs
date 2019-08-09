using System.Collections.Generic;
using System.Linq;
using Entities = Moneteer.Domain.Entities;

namespace Moneteer.Backend.Extensions
{
    public static class RecentTransactionByEnvelopeExtensions
    {
        public static Models.RecentTransactionByEnvelope ToModel(this Entities.RecentTransactionByEnvelope entity)
        {
            if (entity == null) return null;

            return new Models.RecentTransactionByEnvelope
            {
                EnvelopeId = entity.EnvelopeId,
                Amount = entity.Amount,
                Date = entity.Date,
                Payee = entity.Payee
            };
        }

        public static Entities.RecentTransactionByEnvelope ToEntity(this Models.RecentTransactionByEnvelope model)
        {
            if (model == null) return null;

            return new Entities.RecentTransactionByEnvelope
            {
                EnvelopeId = model.EnvelopeId,
                Amount = model.Amount,
                Date = model.Date,
                Payee = model.Payee
            };
        }

        public static IEnumerable<Models.RecentTransactionByEnvelope> ToModels(this IEnumerable<Entities.RecentTransactionByEnvelope> entities)
        {
            if (entities == null) return null;

            return entities.Select(t => t.ToModel());
        }

        public static IEnumerable<Entities.RecentTransactionByEnvelope> ToEntities(this IEnumerable<Models.RecentTransactionByEnvelope> models)
        {
            if (models == null) return null;

            return models.Select(t => t.ToEntity());
        }
    }
}
