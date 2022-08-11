using Microsoft.AspNetCore.Mvc;

namespace TwilightSparkle.PapersPlease.Api.Controllers.Weather
{
    [Route("api/weather")]
    [ApiController]
    public sealed class WeatherController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Weather> Get()
        {
            return new Weather
            {
                Status = "Some weather"
            };
        }



        public sealed class Weather
        {
            public string Status { get; set; }
        }
    }
}