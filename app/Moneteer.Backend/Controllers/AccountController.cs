using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moneteer.Backend.Managers;
using Moneteer.Backend.Services;
using Moneteer.Models;
using System;
using System.Threading.Tasks;

namespace Moneteer.Backend.Controllers
{
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
            return HandleExceptions(() => 
            {
                var userId = GetCurrentUserId();

                return _accountManager.Update(account, userId);
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
