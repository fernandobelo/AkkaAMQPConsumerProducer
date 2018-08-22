using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Akka.Actor;
using Akka.DI;
using Akka.DI.Core;
using Akka.DI.AutoFac;

using AkkaAMQPConsumerProducer.Core.Commands;
using AkkaAMQPConsumerProducer.Core.Events;
using AkkaAMQPConsumerProducer.Infrastructure.Options;

namespace AkkaAMQPConsumerProducer.Infrastructure.Actors
{
    public class BrokerCoordinatorActor : ReceiveActor
    {
        private readonly ILogger<BrokerCoordinatorActor> _logger;

        public BrokerCoordinatorActor(ILogger<BrokerCoordinatorActor> logger, IOptions<BrokerOptions> options)
        {
            _logger = logger;

            _logger.LogInformation("Started coordinator actor. Starting children.");

            var _receiverActor = Context.ActorOf(Context.DI().Props<BrokerReceiverActor>(), "broker-receiver-actor");
            var _senderActor = Context.ActorOf(Context.DI().Props<BrokerSenderActor>(),"broker-sender-actor");

            Receive<SendAliveMessage>(msg =>
            {
                _logger.LogDebug("Got SendAliveMessage. Sending I'm alive message");
                Self.Tell(new SendMessage($"Hello FROM {Sender.Path.Uid}. Random number: {new Random().Next()}"));
            });

            Receive<SendMessage>(msg =>
            {
                _logger.LogDebug("Got SendMessage. Forwarding to sender actor");
                _senderActor.Forward(msg);
            });

            Receive<MessageReceived>(msg =>
            {
                _logger.LogInformation($"Message Received. {msg.Payload}");
            });
        }
    }
}
