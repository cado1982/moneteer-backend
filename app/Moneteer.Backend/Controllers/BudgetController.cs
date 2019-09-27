
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moneteer.Backend.Managers;
using Moneteer.Backend.Services;
using Moneteer.Models;

namespace Moneteer.Backend.Controllers
{
    [Authorize]
    public class BudgetController : BaseController<BudgetController>
    {
        private readonly IBudgetManager _budgetManager;
        private readonly ILogger<BudgetController> _logger;

        public BudgetController(IBudgetManager budgetManager, ILogger<BudgetController> logger, IUserInfoService userInfoService)
            :base(logger, userInfoService)
        {
            _budgetManager = budgetManager;
            _logger = logger;
        }

        [HttpGet("api/budget")]
        public Task<IActionResult> GetForUser()
        {
            return HandleExceptions(async () =>
            {
                var userId = GetCurrentUserId();

                var budgets = await _budgetManager.GetAllForUser(userId);

                if (budgets.Any())
                {
                    return budgets;
                }
                else
                {
                    var budget = new Budget
                    {
                        Currency = new Currency
                        {
                            Code = "USD"
                        },
                        CurrencyFormat = new CurrencyFormat
                        {
                            DecimalPlaces = 2,
                            DecimalSeparator = ".",
                            ThousandsSeparator = ","
                        },
                        CurrencySymbolLocation = SymbolLocation.Before,
                        DateFormat = "dd/MM/yyyy",
                        Name = "My Budget",
                    };

                    var newBudget = await _budgetManager.Create(budget, userId);
                    return new List<Budget> { newBudget };
                }
            });
        }

        [HttpGet("api/budget/{budgetId:guid}")]
        public Task<IActionResult> Get(Guid budgetId)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _budgetManager.Get(budgetId, userId);
            });
        }

        [HttpPost("api/budget")]
        public Task<IActionResult> Post([FromBody] Budget budget)
        {
            if (!ModelState.IsValid)
            {
                return Task.FromResult((IActionResult)BadRequest(ModelState));
            }

            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _budgetManager.Create(budget, userId);
            });
        }

        //[HttpPut("api/budget")]
        //public Task<IActionResult> Put([FromBody] Budget budget)
        //{
        //    return HandleExceptions(() =>
        //    {
        //        var userId = GetCurrentUserId();

        //        throw new NotImplementedException();
        //    });
        //}

        [HttpDelete("api/budget/{budgetId:guid}")]
        public Task<IActionResult> Delete(Guid budgetId)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _budgetManager.Delete(budgetId, userId);
            });
        }
    }
}
