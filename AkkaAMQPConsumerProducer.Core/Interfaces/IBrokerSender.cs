using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaAMQPConsumerProducer.Core.Interfaces
{
    public interface IBrokerSender
    {
        void Publish(string msg);
    }
}
