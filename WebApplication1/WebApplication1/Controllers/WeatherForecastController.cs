using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];
    private static WeatherForecast[] ListWeatherForecast;

    private  ILogger<WeatherForecastController> logger;

    public WeatherForecastController( ILogger<WeatherForecastController> logger)
    {
        this.logger = logger;


        ListWeatherForecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();


    }
/// <summary>
/// return all weather forecast
/// </summary>
/// <returns></returns> <summary>
/// 
/// </summary>
/// <returns></returns>
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        logger.LogInformation("Retrieving list of weather forecasts.");
        return ListWeatherForecast;
    }
/// <summary>
///  return weather forecast by position
/// </summary>
/// <param name="id"></param>
/// <returns></returns> <summary>
/// 
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
    [HttpGet()]
    [Route("{id}")]
    public ActionResult<WeatherForecast> GetByPosition(int id)
    {
        if(id>5 || id<0)
        {
           return BadRequest();
        }
        return Ok (ListWeatherForecast[id]);
    }
}
