using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moneteer.Backend.Managers;
using Moneteer.Backend.Services;
using Moneteer.Models;
using Moneteer.Models.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Moneteer.Backend.Controllers
{
    public class EnvelopeController : BaseController<EnvelopeController>
    {
        private readonly ILogger<EnvelopeController> _logger;
        private readonly IEnvelopeManager _envelopeManager;

        public EnvelopeController(
            ILogger<EnvelopeController> logger,
            IEnvelopeManager envelopeManager, 
            IUserInfoService userInfoService)
                :base(logger, userInfoService)
        {
            _logger = logger;
            _envelopeManager = envelopeManager;
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
            return HandleExceptions(() =>
            {
                if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided", nameof(budgetId));

                var userId = GetCurrentUserId();

                return _envelopeManager.GetEnvelopeCategories(budgetId, userId);
            });
        }

        [HttpPost]
        [Route("api/budget/{budgetId}/envelopes/category")]
        public Task<IActionResult> CreateEnvelopeCategory(Guid budgetId, [FromBody] EnvelopeCategory envelopeCategory)
        {
            return HandleExceptions(() =>
            {
                if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided", nameof(budgetId));

                var userId = GetCurrentUserId();

                return _envelopeManager.CreateEnvelopeCategory(budgetId, envelopeCategory, userId);
            });
        }

        [HttpPost]
        [Route("api/envelopes")]
        public Task<IActionResult> CreateEnvelope([FromBody] Envelope envelope)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _envelopeManager.CreateEnvelope(envelope, userId);
            });
        }
        
        [HttpPost]
        [Route("api/envelopes/{fromEnvelopeId}/movebalance")]
        public Task<IActionResult> MoveBalance(Guid fromEnvelopeId, [FromBody] EnvelopeBalanceTarget[] targets)
        {
            return HandleExceptions(() =>
            {
                if (fromEnvelopeId == Guid.Empty) throw new ArgumentException("envelopeId must be provided", nameof(fromEnvelopeId));

                var userId = GetCurrentUserId();

                return _envelopeManager.MoveEnvelopeBalance(fromEnvelopeId, targets.ToList(), userId);
            });
        }

        [HttpDelete]
        [Route("api/envelopes/{envelopeId}")]
        public Task<IActionResult> DeleteEnvelope(Guid envelopeId)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _envelopeManager.DeleteEnvelope(envelopeId, userId);
            });
        }

        [HttpPut]
        [Route("api/envelopes")]
        public Task<IActionResult> UpdateEnvelope(Envelope envelope)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _envelopeManager.UpdateEnvelope(envelope, userId);
            });
        }

        [HttpPut]
        [Route("api/envelopes/{envelopeId}/hide")]
        public Task<IActionResult> HideEnvelope(Guid envelopeId)
        {
            return HandleExceptions(() => 
            {
                var userId = GetCurrentUserId();

                return _envelopeManager.HideEnvelope(envelopeId, userId);
            });
        }

        [HttpPut]
        [Route("api/envelopes/{envelopeId}/show")]
        public Task<IActionResult> ShowEnvelope(Guid envelopeId)
        {
            return HandleExceptions(() => 
            {
                var userId = GetCurrentUserId();

                return _envelopeManager.ShowEnvelope(envelopeId, userId);
            });
        }

        [HttpPut]
        [Route("api/envelopeCategories/{envelopeCategoryId}/show")]
        public Task<IActionResult> ShowEnvelopeCategory(Guid envelopeCategoryId)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _envelopeManager.ShowEnvelopeCategory(envelopeCategoryId, userId);
            });
        }

        [HttpPut]
        [Route("api/envelopeCategories/{envelopeCategoryId}/hide")]
        public Task<IActionResult> HideEnvelopeCategory(Guid envelopeCategoryId)
        {
            return HandleExceptions(() =>
            {
                var userId = GetCurrentUserId();

                return _envelopeManager.HideEnvelopeCategory(envelopeCategoryId, userId);
            });
        }
    }
}
