using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Moneteer.Backend.Controllers
{
    [AllowAnonymous]
    public class HealthCheckController : Controller
    {
        [Route("healthcheck")]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
