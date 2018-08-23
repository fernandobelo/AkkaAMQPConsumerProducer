using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

using AkkaAMQPConsumerProducer.Core.Interfaces;
using AkkaAMQPConsumerProducer.Infrastructure.Options;

namespace AkkaAMQPConsumerProducer.Infrastructure.Services
{
    public class BrokerSender : IBrokerSender
    {
        private readonly IModel _channel;
        private readonly BrokerOptions _options;

        public BrokerSender(IOptions<BrokerOptions> options)
        {
            _options = options.Value;

            _channel = BindChannel();
        }

        private IModel BindChannel()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _options.HostName,
                    UserName = _options.UserName,
                    Password = _options.Password
                };

                if (!String.IsNullOrEmpty(_options.VirtualHost)) factory.VirtualHost = _options.VirtualHost;

                var connection = factory.CreateConnection();

                return connection.CreateModel();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Publish(string msg)
        {
            try
            {
                _channel.BasicPublish(_options.ExchangeName, "", null, Encoding.UTF8.GetBytes(msg));
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

}
