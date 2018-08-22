using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Akka.Actor;

using RabbitMQ.Client;

using AkkaAMQPConsumerProducer.Core.Commands;
using AkkaAMQPConsumerProducer.Infrastructure.Options;

namespace AkkaAMQPConsumerProducer.Infrastructure.Actors
{
    public class BrokerSenderActor : ReceiveActor
    {
        private readonly ILogger<BrokerSenderActor> _logger;
        private readonly BrokerOptions _options;

        public BrokerSenderActor(ILogger<BrokerSenderActor> logger, IOptions<BrokerOptions> options)
        {
            _logger = logger;
            _options = options.Value;

            _logger.LogDebug("Starting broker sender actor");

            var factory = new ConnectionFactory() { HostName = _options.HostName, VirtualHost = _options.VirtualHost, UserName = _options.UserName, Password = _options.Password };
            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            Receive<SendMessage>(msg =>
            {
                _logger.LogDebug("Got SendMessage");

                channel.BasicPublish(_options.ExchangeName, "", null, Encoding.UTF8.GetBytes(msg.Payload));
            });
        }
    }
}
