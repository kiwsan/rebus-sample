using System;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Persistence.InMem;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;

namespace Publisher.AppConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Service registration pipeline...
            var services = new ServiceCollection();

            // 1.1. Configure Rebus
            services.AddRebus(configure => configure
                .Logging(l => l.ColoredConsole())
                .Transport(t => t.UsePostgreSql("server=localhost; database=rebus; user id=postgres; password=postgres; maximum pool size=30", "messages", "Publisher.AppConsole"))
                .Subscriptions(x => x.StoreInPostgres("server=localhost; database=rebus; user id=postgres; password=postgres; maximum pool size=30", "subscriptions")));

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

                // 3.2. Begin the domain work for the application}
                var bus = provider.GetRequiredService<IBus>();

                var startupTime = DateTime.Now;

                while (true)
                {
                    Console.WriteLine(@"a) Publish string
b) Publish DateTime
c) Publish TimeSpan
q) Quit");

                    var keyChar = char.ToLower(Console.ReadKey(true).KeyChar);

                    switch (keyChar)
                    {
                        case 'a':
                            bus.Publish(new StringMessage("Hello there, I'm a publisher!")).Wait();
                            break;

                        case 'b':
                            bus.Publish(new DateTimeMessage(DateTime.Now)).Wait();
                            break;

                        case 'c':
                            bus.Publish(new TimeSpanMessage(DateTime.Now - startupTime)).Wait();
                            break;

                        case 'q':
                            goto consideredHarmful;

                        default:
                            Console.WriteLine("There's no option ({0})", keyChar);
                            break;
                    }
                }

            consideredHarmful:;
                Console.WriteLine("Quitting!");
            }

        }
    }
}