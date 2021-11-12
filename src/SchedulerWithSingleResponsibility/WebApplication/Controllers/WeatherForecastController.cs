using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AddinEngine.Abstract;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IServiceProvider _services;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IServiceProvider services, ILogger<WeatherForecastController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var taskQueue = _services.GetRequiredService<ObservableConcurrentQueue<string>>();
            taskQueue.Enqueue("abc");
            
            var jobId = BackgroundJob.Schedule(
                () => AnyBackgroundService.RegisterWrapper.RegisterToRun("scheduled job or any json string"),
                TimeSpan.FromSeconds(5));
            
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}