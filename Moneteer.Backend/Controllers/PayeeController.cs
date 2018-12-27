using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moneteer.Backend.Managers;
using Moneteer.Backend.Services;
using Moneteer.Models;
using System;
using System.Threading.Tasks;

namespace Moneteer.Backend.Controllers
{
    [Authorize]
    public class PayeeController : BaseController
    {
        private readonly IPayeeManager _payeeManager;

        public PayeeController(IPayeeManager payeeManager, IUserInfoService userInfoService)
            :base(userInfoService)
        {
            _payeeManager = payeeManager;
        }

        [HttpGet]
        [Route("api/budget/{budgetId}/payees")]
        public Task<IActionResult> GetAllByBudget(Guid budgetId)
        {
            return HandleExceptions(() => 
            {
                var userId = GetCurrentUserId();

                return _payeeManager.GetAllForBudget(budgetId, userId);
            });
        }

        [HttpPut]
        [Route("api/payee")]
        public Task<IActionResult> Put([FromBody] Payee payee)
        {
            if (!ModelState.IsValid)
            {
                return Task.FromResult((IActionResult)BadRequest(ModelState));
            }

            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _payeeManager.UpdatePayee(payee, userId);
            });
        }

        [HttpDelete]
        [Route("api/payee/{payeeId}")]
        public Task<IActionResult> Delete(Guid payeeId)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _payeeManager.DeletePayee(payeeId, userId);
            });
        }
    }
}
