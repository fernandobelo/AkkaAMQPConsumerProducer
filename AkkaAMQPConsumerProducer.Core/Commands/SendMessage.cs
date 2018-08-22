using System;
namespace AkkaAMQPConsumerProducer.Core.Commands
{
    public class SendMessage
    {
        public string Payload { get; internal set; }

        public SendMessage(string payload)
        {
            Payload = payload;
        }
    }
}
