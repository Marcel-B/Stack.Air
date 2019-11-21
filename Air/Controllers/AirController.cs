using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.b_velop.stack.Air.Constants;
using com.b_velop.stack.Air.Models;
using com.b_velop.stack.Air.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace com.b_velop.stack.Air.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AirController : Controller
    {
        private readonly IUploadService _service;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AirController> _logger;

        public AirController(
            IUploadService service,
            IMemoryCache cache,
            ILogger<AirController> logger
        )
        {
            _service = service;
            _cache = cache;
            _logger = logger;
        }

        // GET: api/air
        [HttpGet(Name = "GetLastValues")]
        [ProducesResponseType(typeof(Dictionary<string, double>), 200)]
        [ProducesResponseType(500)]
        public IActionResult Get()
        {
            if (!_cache.TryGetValue(Memory.Values, out Dictionary<string, double> values))
            {
                return new StatusCodeResult(500);
            }
            return new JsonResult(values);
        }

        // POST api/air
        [HttpPost()]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Post(
            AirDto value)
        {
            try
            {
                if(value == null)
                {
                    _logger.LogWarning(2442, $"No Airdata received.");
                    return new StatusCodeResult(500);
                }
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