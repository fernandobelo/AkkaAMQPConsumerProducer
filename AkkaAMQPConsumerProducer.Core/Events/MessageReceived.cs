using System;

namespace AkkaAMQPConsumerProducer.Core.Events
{
    public class MessageReceived
    {
        public string Payload { get; internal set; }

        public MessageReceived(string payload)
        {
            Payload = payload;
        }
    }
}
