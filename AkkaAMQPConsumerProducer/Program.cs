using System;
using System.Threading.Tasks;


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.AutoFac;

using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

using AkkaAMQPConsumerProducer.Core.Commands;
using AkkaAMQPConsumerProducer.Core.Interfaces;
using AkkaAMQPConsumerProducer.Infrastructure.Actors;
using AkkaAMQPConsumerProducer.Infrastructure.Options;
using AkkaAMQPConsumerProducer.Infrastructure.Services;

namespace AkkaAMQPConsumerProducer
{
    class Program
    {
        private static IContainer _container;
 
        public static void Main(string[] args)
        {
            // loading configuration
            var _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            // setup services (DI and logger)
            SetupServices(_config);

            // setup actor system
            SetupActorSystem();

            Console.ReadKey();
        }

        private static void SetupServices(IConfigurationRoot _config)
        {
            // configure our logger (serilog)
            var loggerConfig = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.FromLogContext()
               .ReadFrom.Configuration(_config)
               .CreateLogger();

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddOptions();
            services.AddSingleton<IBrokerSender, BrokerSender>();

            services.Configure<BrokerOptions>(_config.GetSection("RabbitMQ"));

            // Initialize Autofac
            var builder = new ContainerBuilder();


            builder.RegisterType<BrokerCoordinatorActor>();
            builder.RegisterType<BrokerReceiverActor>();
            builder.RegisterType<BrokerSenderActor>();


            // Use the Populate method to register services which were registered
            // to IServiceCollection
            builder.Populate(services);

            // Build the final container
            _container = builder.Build();

            var loggerFactory = _container.Resolve<ILoggerFactory>();
            loggerFactory.AddSerilog(loggerConfig);
        }

        private static void SetupActorSystem()
        {
            var _actorSystem = Akka.Actor.ActorSystem.Create("MainActorSystem");

            var propsResolver = new AutoFacDependencyResolver(_container, _actorSystem);

            var _coordinatorActor = _actorSystem.ActorOf(_actorSystem.DI().Props<BrokerCoordinatorActor>(), "brooker-coordinator");

            _actorSystem.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5), _coordinatorActor, new SendAliveMessage(), _coordinatorActor);
        }
    }
}
