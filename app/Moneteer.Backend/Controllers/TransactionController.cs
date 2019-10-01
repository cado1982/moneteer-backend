using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moneteer.Backend.Managers;
using Moneteer.Backend.Services;
using Moneteer.Domain;
using Moneteer.Models;
using Npgsql;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Moneteer.Backend.Controllers
{
    public class TransactionController : BaseController<TransactionController>
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionManager _transactionManager;

        public TransactionController(ILogger<TransactionController> logger,  ITransactionManager transactionManager, IUserInfoService userInfoService)
            :base(logger, userInfoService)
        {
            _logger = logger;
            _transactionManager = transactionManager;
        }

        [HttpGet]
        [Route("api/budget/{budgetId}/transactions")]
        public Task<IActionResult> GetAllForBudget(Guid budgetId)
        {
            return HandleExceptions(() => 
            {
                var userId = GetCurrentUserId();

                return _transactionManager.GetAllForBudget(budgetId, userId);
            });
        }

        [HttpGet]
        [Route("api/account/{accountId}/transactions")]
        public Task<IActionResult> GetAllForAccount(Guid accountId)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _transactionManager.GetAllForAccount(accountId, userId);
            });
        }

        [HttpPost]
        [Route("api/transaction")]
        public Task<IActionResult> Post([FromBody] CreateTransactionRequest request)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                var transaction = new Transaction();
                transaction.Account = new Account { Id = request.AccountId };
                transaction.Date = request.Date;
                transaction.Assignments = request.Assignments;
                transaction.Description = request.Description;
                transaction.IsCleared = request.IsCleared;
                transaction.IsReconciled = false;

                return _transactionManager.CreateTransaction(transaction, userId);
            });
        }

        [HttpPut]
        [Route("api/transaction")]
        public Task<IActionResult> Put([FromBody] Transaction transaction)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _transactionManager.UpdateTransaction(transaction, userId);
            });
        }

        [HttpPut]
        [Route("api/transaction/{transactionId}/setCleared")]
        public Task<IActionResult> SetCleared(Guid transactionId, [FromBody] bool isCleared)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _transactionManager.SetTransactionIsCleared(transactionId, isCleared, userId);
            });
        }

        [HttpDelete]
        [Route("api/transaction")]
        public Task<IActionResult> Delete([FromBody] Guid[] transactionIds)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _transactionManager.DeleteTransactions(transactionIds.ToList(), userId);
            });
        }

        [HttpGet]
        [Route("api/budget/{budgetId}/recenttransactions")]
        public Task<IActionResult> GetRecentTransactions(Guid budgetId, [FromQuery] int numberOfTransactions = 10)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _transactionManager.GetRecentTransactionsByEnvelope(budgetId, numberOfTransactions, userId);
            });
        }
    }
}
