using System;
using System.Threading.Tasks;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;

namespace Subscriber1.AppConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Service registration pipeline...
            var services = new ServiceCollection();
            services.AutoRegisterHandlersFromAssemblyOf<Handler>();

            // 1.1. Configure Rebus
            services.AddRebus(configure => configure
                .Logging(l => l.ColoredConsole())
                .Transport(t => t.UsePostgreSql("server=localhost; database=rebus; user id=postgres; password=postgres; maximum pool size=30", "messages", "Subscriber1.AppConsole"))
                .Routing(r => r.TypeBased().MapAssemblyOf<StringMessage>("Publisher.AppConsole"))
                .Options(o =>
                    {
                        o.SetNumberOfWorkers(10);
                        o.SetMaxParallelism(20);
                    }));

            // 1.2. Potentially add more service registrations for the application, some of which
            //      could be required by handlers.

            // 2. Application starting pipeline...
            // Make sure we correctly dispose of the provider (and therefore the bus) on application shutdown
            using (var provider = services.BuildServiceProvider())
            {
                // 3. Application started pipeline...

                // 3.1. Now application is running, lets trigger the 'start' of Rebus.
                provider.UseRebus();
                //optionally...
                //provider.UseRebus(async bus => await bus.Subscribe<Message1>());

                // 3.2. Begin the domain work for the application
                var bus = provider.GetRequiredService<IBus>();

                bus.Subscribe<StringMessage>().Wait();
                bus.Subscribe<DateTimeMessage>().Wait();
                bus.Subscribe<TimeSpanMessage>().Wait();

                Console.WriteLine("This is Subscriber 1");
                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
                Console.WriteLine("Quitting...");
            }
        }
    }
}

public class Handler : IHandleMessages<StringMessage>, IHandleMessages<DateTimeMessage>, IHandleMessages<TimeSpanMessage>
{
    public async Task Handle(StringMessage message)
    {
        Console.WriteLine("Got string: {0}", message.Text);
    }

    public async Task Handle(DateTimeMessage message)
    {
        Console.WriteLine("Got DateTime: {0}", message.DateTime);
    }

    public async Task Handle(TimeSpanMessage message)
    {
        Console.WriteLine("Got TimeSpan: {0}", message.TimeSpan);
    }
}
