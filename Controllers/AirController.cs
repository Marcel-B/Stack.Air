using System;
using System.Threading.Tasks;
using com.b_velop.stack.Air.Services;
using com.b_velop.stack.Classes.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace com.b_velop.stack.Air.Controllers
{
    [Route("api/[controller]")]
    public class AirController : Controller
    {
        private ILogger<AirController> _logger;
        private IUploadService _service;

        public AirController(
            IUploadService service,
            ILogger<AirController> logger
        )
        {
            _logger = logger;
            _service = service;
        }

        // GET: api/values
        [HttpGet]
        public IActionResult Get()
            => Ok();

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody]AirdataDto value)
        {
            try
            {
                await _service.UploadAsync(value, DateTimeOffset.Now);
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(1111, ex, $"Error occurred while uploading '{value}'.", value);
                return new StatusCodeResult(500);
            }
        }
    }
}