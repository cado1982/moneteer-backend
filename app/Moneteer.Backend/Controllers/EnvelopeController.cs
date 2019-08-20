using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moneteer.Backend.Managers;
using Moneteer.Backend.Services;
using Moneteer.Models;
using Moneteer.Models.Validation;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Moneteer.Backend.Controllers
{
    [Authorize]
    public class EnvelopeController : BaseController<EnvelopeController>
    {
        private readonly ILogger<EnvelopeController> _logger;
        private readonly IEnvelopeManager _envelopeManager;
        private readonly EnvelopeCategoryValidationStrategy _masterCategoryValidationStrategy;
        private readonly EnvelopeValidationStrategy _childCategoryValidationStrategy;

        public EnvelopeController(
            ILogger<EnvelopeController> logger,
            IEnvelopeManager envelopeManager, 
            EnvelopeCategoryValidationStrategy masterCategoryValidationStrategy, 
            EnvelopeValidationStrategy childCategoryValidationStrategy,
            IUserInfoService userInfoService)
                :base(logger, userInfoService)
        {
            _logger = logger;
            _envelopeManager = envelopeManager;
            _masterCategoryValidationStrategy = masterCategoryValidationStrategy;
            _childCategoryValidationStrategy = childCategoryValidationStrategy;
        }

        [HttpGet]
        [Route("api/budget/{budgetId}/envelopes")]
        public Task<IActionResult> GetAllForBudget(Guid budgetId)
        {
            return HandleExceptions(() =>
            {
                if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided", nameof(budgetId));

                var userId = GetCurrentUserId();

                return _envelopeManager.GetEnvelopes(budgetId, userId);
            });
        }

        [HttpGet]
        [Route("api/budget/{budgetId}/envelopes/categories")]
        public Task<IActionResult> GetAllEnvelopeCategoriesForBudget(Guid budgetId)
        {
            return HandleExceptions(async () =>
            {
                if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided", nameof(budgetId));

                var userId = GetCurrentUserId();

                var result = await _envelopeManager.GetEnvelopeCategories(budgetId, userId);

                return result;
            });
        }

        [HttpPost]
        [Route("api/budget/{budgetId}/envelopes/category")]
        public Task<IActionResult> CreateEnvelopeCategory(Guid budgetId, [FromBody] EnvelopeCategory envelopeCategory)
        {
            return HandleExceptions(async () =>
            {
                if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided", nameof(budgetId));

                var userId = GetCurrentUserId();

                return await _envelopeManager.CreateEnvelopeCategory(budgetId, envelopeCategory, userId);
            });
        }

        [HttpPost]
        [Route("api/budget/{budgetId}/envelopes")]
        public Task<IActionResult> CreateEnvelope(Guid budgetId, [FromBody] Envelope envelope)
        {
            return HandleExceptions(async () =>
            {
                if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided", nameof(budgetId));

                var userId = GetCurrentUserId();

                return await _envelopeManager.CreateEnvelope(budgetId, envelope, userId);
            });
        }

        [HttpPost]
        [Route("api/envelopes/{fromEnvelopeId}/movebalance")]
        public Task<IActionResult> MoveBalance(Guid fromEnvelopeId, [FromBody] EnvelopeBalanceTarget[] targets)
        {
            return HandleExceptions(async () =>
            {
                if (fromEnvelopeId == Guid.Empty) throw new ArgumentException("envelopeId must be provided", nameof(fromEnvelopeId));

                var userId = GetCurrentUserId();

                await _envelopeManager.MoveEnvelopeBalance(fromEnvelopeId, targets.ToList(), userId);
            });
        }

        [HttpDelete]
        [Route("api/envelopes/{envelopeId}")]
        public Task<IActionResult> DeleteEnvelope(Guid envelopeId)
        {
            return HandleExceptions(async () =>
            {
                var userId = GetCurrentUserId();

                await _envelopeManager.DeleteEnvelope(envelopeId, userId);
            });
        }
    }
}
