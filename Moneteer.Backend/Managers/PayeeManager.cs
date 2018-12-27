using Moneteer.Backend.Extensions;
using Moneteer.Domain.Guards;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using Moneteer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moneteer.Backend.Managers
{
    public class PayeeManager : BaseManager, IPayeeManager
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly IPayeeRepository _payeeRepository;

        public PayeeManager(IConnectionProvider connectionProvider, IPayeeRepository payeeRepository, Guards guards)
            :base(guards)
        {
            _connectionProvider = connectionProvider;
            _payeeRepository = payeeRepository;
        }
        
        public async Task DeletePayee(Guid payeeId, Guid userId)
        {
            await GuardPayee(payeeId, userId);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await _payeeRepository.DeletePayee(payeeId, conn);
            }
        }

        public async Task<List<Payee>> GetAllForBudget(Guid budgetId, Guid userId)
        {
            await GuardBudget(budgetId, userId);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                var payees = await _payeeRepository.GetAllForBudget(budgetId, conn);

                var models = payees.ToModels();

                return models.ToList();
            }
        }

        public async Task UpdatePayee(Payee payee, Guid userId)
        {
            await GuardPayee(payee.Id, userId);

            var entity = payee.ToEntity();

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await _payeeRepository.UpdatePayee(entity, conn);
            }
        }
    }
}
