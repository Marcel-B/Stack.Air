using System;
using System.Threading.Tasks;
using com.b_velop.stack.Air.Services;
using com.b_velop.stack.Classes.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace com.b_velop.stack.Air.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AirController : Controller
    {
        private readonly ILogger<AirController> _logger;
        private readonly IUploadService _service;

        public AirController(
            IUploadService service,
            ILogger<AirController> logger
        )
        {
            _logger = logger;
            _service = service;
        }

        // GET: api/air
        [HttpGet]
        public IActionResult Get()
            => Ok();

        // POST api/air
        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody]AirdataDto value)
        {
            try
            {
                await _service.UploadAsync(value, DateTimeOffset.Now);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(2442, ex, $"Error occurred while uploading value '{value}'.", value);
                return new StatusCodeResult(500);
            }
        }
    }
}