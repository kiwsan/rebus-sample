using System;
using System.Threading.Tasks;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;

namespace Publisher.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IBus _bus;
        public HomeController(IBus bus)
        {
            _bus = bus;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var startupTime = DateTime.Now;

            await _bus.Publish(new StringMessage("Hello there, I'm a publisher!"));
            await _bus.Publish(new DateTimeMessage(DateTime.Now));
            await _bus.Publish(new TimeSpanMessage(DateTime.Now - startupTime));

            return Ok(startupTime);
        }
    }
}