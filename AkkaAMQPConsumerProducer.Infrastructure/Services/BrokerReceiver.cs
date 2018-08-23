using System;
using System.Collections.Generic;
using System.Text;

using Akka.Actor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using AkkaAMQPConsumerProducer.Core.Events;
using AkkaAMQPConsumerProducer.Core.Interfaces;
using AkkaAMQPConsumerProducer.Infrastructure.Options;

namespace AkkaAMQPConsumerProducer.Infrastructure.Services
{
    public class BrokerReceiver : IBrokerReceiver
    {
        private IModel _channel;
        private readonly BrokerOptions _options;
        private readonly IActorRef _self;

        public BrokerReceiver(IActorRef self, BrokerOptions options, string queueName)
        {
            _options = options;
            _self = self;

            // bind channel to the broker
            _channel = BindChannel();

            // setup queue and exchange
            SetupQueue(queueName);

            // setup consumer
            SetupConsumer(queueName);

        }

        private void SetupQueue(string queueName)
        {
            // declare exchange (in case it is not present already)
            _channel.ExchangeDeclare(_options.ExchangeName, "fanout");


            // declare queue for this receiver
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: true, arguments: null);


            // bind to the exchange
            _channel.QueueBind(queue: queueName, exchange: _options.ExchangeName, routingKey: "");
        }

        private IModel BindChannel()
        {
            var factory = new ConnectionFactory() { HostName = _options.HostName, UserName = _options.UserName, Password = _options.Password };

            if (!String.IsNullOrEmpty(_options.VirtualHost)) factory.VirtualHost = _options.VirtualHost;

            var connection = factory.CreateConnection();

            return connection.CreateModel();
        }

        private void SetupConsumer(string queueName)
        {
            // create the consumer
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                _self.Tell(new MessageReceived(message));
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        }
    }

}
