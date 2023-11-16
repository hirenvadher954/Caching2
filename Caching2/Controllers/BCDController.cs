using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Caching2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BCDController : ControllerBase
    {
        private static readonly string[] TestingNames = new[]
        {
        "Hiren", "Lal", "Raj", "Ravi", "Rajesh"
    };

        private readonly ILogger<BCDController> _logger;
        private readonly IDistributedCache _cache;


        public BCDController(ILogger<BCDController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet(Name = "GetTest")]
        public async Task<IEnumerable<SampModel>> Get()
        {
            const string cacheKey = "WeatherForecast";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (cachedData != null)
            {
                return JsonSerializer.Deserialize<IEnumerable<SampModel>>(cachedData);
            }

            var samps
                = Enumerable.Range(1, 5).Select(index => new SampModel
            {
                Name = TestingNames[index]
            })
            .ToArray();

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(samps), options);

            return samps;
        }
    }
}