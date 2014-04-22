using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POC_WindowsAzureServiceBus.Azure;

namespace POC_WindowsAzureServiceBusReceiver
{
    class Program
    {
        private static AzureServiceBus azBus;

        static void Main(string[] args)
        {
            azBus = new AzureServiceBus();
            azBus.readMessageFromBus("test/msgpump");
        }
    }
}
