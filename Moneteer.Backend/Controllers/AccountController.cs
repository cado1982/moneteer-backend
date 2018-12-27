using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moneteer.Backend.Managers;
using Moneteer.Backend.Services;
using Moneteer.Domain;
using Moneteer.Models;
using Npgsql;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Moneteer.Backend.Controllers
{
    [Route("api/account")]
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IAccountManager _accountManager;

        public AccountController(IAccountManager accountManager, IUserInfoService userInfoService)
            :base(userInfoService)
        {
            _accountManager = accountManager;
        }

        [HttpGet] // api/account?budgetId=XXX
        public Task<IActionResult> GetForBudget(Guid budgetId)
        {
            return HandleExceptions(() => 
            {
                var userId = GetCurrentUserId();

                return _accountManager.GetAllForBudget(budgetId, userId);
            });
        }

        [HttpPost] // api/account/budgetId=XXX
        public Task<IActionResult> Post(Guid budgetId, [FromBody] Account account)
        {
            if (!ModelState.IsValid)
            {
                return Task.FromResult((IActionResult)BadRequest(ModelState));
            }

            return HandleExceptions(() => 
            {
                var userId = GetCurrentUserId();
                account.BudgetId = budgetId;

                return  _accountManager.Create(account, userId);
            });
        }

        [HttpPut] // api/account
        public Task<IActionResult> Put([FromBody] Account account)
        {
            if (!ModelState.IsValid)
            {
                return Task.FromResult((IActionResult)BadRequest(ModelState));
            }

            return HandleExceptions(async () => 
            {
                var userId = GetCurrentUserId();

                await _accountManager.Update(account, userId);

                return account;
            });
        }

        [HttpDelete("{accountId}")] // api/account/XXX
        public Task<IActionResult> Delete(Guid accountId)
        {
            return HandleExceptions(() => 
            {
                var userId = GetCurrentUserId();

                return _accountManager.Delete(accountId, userId);
            });
        }
    }
}
