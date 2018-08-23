using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Akka.Actor;


using AkkaAMQPConsumerProducer.Core.Events;
using AkkaAMQPConsumerProducer.Core.Interfaces;
using AkkaAMQPConsumerProducer.Infrastructure.Services;
using AkkaAMQPConsumerProducer.Infrastructure.Options;

namespace AkkaAMQPConsumerProducer.Infrastructure.Actors
{
   
    public class BrokerReceiverActor : ReceiveActor
    {
        private readonly ILogger<BrokerReceiverActor> _logger;
        private readonly IBrokerReceiver _broker;
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
