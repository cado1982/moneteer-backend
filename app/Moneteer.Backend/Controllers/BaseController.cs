using System;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moneteer.Backend.Services;
using Moneteer.Domain.Exceptions;

namespace Moneteer.Backend.Controllers
{
    [Authorize(Policy = "Subscriber")]
    public abstract class BaseController<T> : Controller
    {
        protected ILogger<T> Logger { get; }

        protected IUserInfoService UserInfoService { get; }
        
        protected BaseController(ILogger<T> logger, IUserInfoService userInfoService)
        {
            Logger = logger;
            UserInfoService = userInfoService;
        }

        protected Guid GetCurrentUserId()
        {
            var userId = UserInfoService.GetUserId();

            if (userId == null)
            {
                throw new SecurityException("User not found");
            }

            return userId;
        }

        protected async Task<IActionResult> HandleExceptions(Func<Task> task)
        {
            try
            {
                await task();
                return Ok();
            }
            catch (ApplicationException ex)
            {
                Logger.LogDebug(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                Logger.LogError(ex, "Forbidden exception encountered.");
                return StatusCode((int)HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        protected async Task<IActionResult> HandleExceptions<TResult>(Func<Task<TResult>> task)
        {
            try
            {
                var result = await task();
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                Logger.LogDebug(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                Logger.LogError(ex, "Forbidden exception encountered.");
                return StatusCode((int)HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
