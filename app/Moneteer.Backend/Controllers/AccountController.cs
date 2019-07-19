using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    [Authorize]
    public class AccountController : BaseController<AccountController>
    {
        private readonly IAccountManager _accountManager;

        public AccountController(ILogger<AccountController> logger, IAccountManager accountManager, IUserInfoService userInfoService)
            :base(logger, userInfoService)
        {
            _accountManager = accountManager;
        }

        [HttpGet]
        [Route("api/budget/{budgetId}/accounts")]
        public Task<IActionResult> GetForBudget(Guid budgetId)
        {
            return HandleExceptions(() => 
            {
                var userId = GetCurrentUserId();

                return _accountManager.GetAllForBudget(budgetId, userId);
            });
        }

        [HttpGet]
        [Route("api/account/{accountId}")]
        public Task<IActionResult> Get(Guid accountId)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _accountManager.Get(accountId, userId);
            });
        }

        [HttpPost]
        [Route("api/account")]
        public Task<IActionResult> Post([FromBody] Account account)
        {
            if (!ModelState.IsValid)
            {
                return Task.FromResult((IActionResult)BadRequest(ModelState));
            }

            return HandleExceptions(() => 
            {
                var userId = GetCurrentUserId();

                return  _accountManager.Create(account, userId);
            });
        }

        [HttpPut]
        [Route("api/account")]
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

        [HttpDelete("api/account/{accountId}")]
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
