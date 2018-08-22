using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Akka.Actor;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using AkkaAMQPConsumerProducer.Core.Events;
using AkkaAMQPConsumerProducer.Infrastructure.Options;

namespace AkkaAMQPConsumerProducer.Infrastructure.Actors
{
    public class BrokerReceiver
    {
        public BrokerReceiver(IActorRef self, BrokerOptions options, string queueName)
        {
            var factory = new ConnectionFactory() { HostName = options.HostName, VirtualHost = options.VirtualHost, UserName = options.UserName, Password = options.Password };
            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // declare exchange (in case it is not present already)
            channel.ExchangeDeclare(options.ExchangeName, "fanout");


            // declare queue for this receiver
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: true, arguments: null);


            // bind to the exchange
            channel.QueueBind(queue: queueName, exchange: options.ExchangeName, routingKey: "");


            // create the consumer
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                self.Tell(new MessageReceived(message));
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
    }

    public class BrokerReceiverActor : ReceiveActor
    {
        private readonly ILogger<BrokerReceiverActor> _logger;
        private readonly BrokerReceiver _broker;
        private readonly BrokerOptions _options;

        public BrokerReceiverActor(ILogger<BrokerReceiverActor> logger, IOptions<BrokerOptions> options)
        {
            _logger = logger;
            _options = options.Value;

            _broker = new BrokerReceiver(Self, _options, Guid.NewGuid().ToString());

            Receive<MessageReceived>(msg =>
            {
                _logger.LogDebug("Got MessageReceived");

                Context.Parent.Tell(msg);
            });
        }
     
    }
}
