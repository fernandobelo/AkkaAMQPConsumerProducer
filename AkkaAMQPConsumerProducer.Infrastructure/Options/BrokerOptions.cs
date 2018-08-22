using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaAMQPConsumerProducer.Infrastructure.Options
{
    public class BrokerOptions
    {
        public BrokerOptions() { }

        public string HostName { get; set; }
        public string VirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ExchangeName { get; set; }

    }
}
