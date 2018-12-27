using System;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moneteer.Backend.Services;

namespace Moneteer.Backend.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IUserInfoService UserInfoService { get; }

        protected BaseController(IUserInfoService userInfoService)
        {
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
                await task().ConfigureAwait(false);
                return Ok();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        protected async Task<IActionResult> HandleExceptions<T>(Func<Task<T>> func) where T : class
        {
            try
            {
                var result = await func().ConfigureAwait(false);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        protected async Task<IActionResult> HandleExceptions(Func<Task<IActionResult>> task)
        {
            try
            {
                var result = await task().ConfigureAwait(false);
                return result;
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
