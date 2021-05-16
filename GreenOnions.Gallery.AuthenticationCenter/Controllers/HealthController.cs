using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GreenOnions.Gallery.AuthenticationCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IConfiguration _iConfiguration;
        public HealthController(IConfiguration iConfiguration, ILogger<HealthController> logger)
        {
            _iConfiguration = iConfiguration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation($"{_iConfiguration["Port"]}实例工作正常");
            return Ok();
        }
    }
}
