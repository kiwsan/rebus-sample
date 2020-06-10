using System;
using System.Threading.Tasks;
using Messages;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Subscriber1.WebApi
{
    public class Handler : IHandleMessages<StringMessage>, IHandleMessages<DateTimeMessage>, IHandleMessages<TimeSpanMessage>
    {
        private readonly ILogger<Handler> _logger;
        public Handler(ILogger<Handler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(StringMessage message)
        {
            _logger.LogInformation("Got string: {0}", message.Text);
        }

        public async Task Handle(DateTimeMessage message)
        {
            _logger.LogInformation("Got DateTime: {0}", message.DateTime);
        }

        public async Task Handle(TimeSpanMessage message)
        {
            _logger.LogInformation("Got TimeSpan: {0}", message.TimeSpan);
        }
    }
}