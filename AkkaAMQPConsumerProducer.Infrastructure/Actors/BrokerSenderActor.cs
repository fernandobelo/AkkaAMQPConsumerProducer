using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Akka.Actor;

using AkkaAMQPConsumerProducer.Core.Commands;
using AkkaAMQPConsumerProducer.Core.Interfaces;
using AkkaAMQPConsumerProducer.Infrastructure.Options;
using AkkaAMQPConsumerProducer.Infrastructure.Services;

namespace AkkaAMQPConsumerProducer.Infrastructure.Actors
{

    public class BrokerSenderActor : ReceiveActor
    {
        private readonly ILogger<BrokerSenderActor> _logger;
        private readonly BrokerOptions _options;
        private IBrokerSender _brokerSender;

        public BrokerSenderActor(ILogger<BrokerSenderActor> logger, IOptions<BrokerOptions> options, IBrokerSender brokerSender)
        {
            try
            {
                _logger = logger;
                _options = options.Value;
                _brokerSender = brokerSender;

                _logger.LogDebug("Starting broker sender actor");

                Receive<SendMessage>(msg =>
                {
                    _logger.LogDebug("Got SendMessage");

                    _brokerSender.Publish(msg.Payload);
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error while starting brokersenderactor");
            }
        }
   }
}
