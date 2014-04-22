using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace POC_WindowsAzureServiceBus.Azure
{
    class AzureServiceBus 
    {

        private static string azureConnectionString = "Endpoint=sb://[NAMESPACE].servicebus.windows.net/;SharedSecretIssuer=owner;SharedSecretValue=[KEY_VALUE]";

        public void sendMessageToServiceBus(string message, Boolean deleteQueWhenExists)
        {
            string qName = "test/msgpump";
            
            prepareQueue(qName, deleteQueWhenExists);

            var client = QueueClient.CreateFromConnectionString(azureConnectionString, qName, ReceiveMode.PeekLock);
            client.Send(new BrokeredMessage(message));

            client.OnMessage((msg) =>
            {

            }, new OnMessageOptions()
            {
                AutoComplete = true,
                MaxConcurrentCalls = 5
            });
        }

        /// <summary>
        /// Check if queue exists within Azure. If it does, delete it.
        /// </summary>
        /// <param name="qName"></param>
        /// <returns></returns>
        private static QueueDescription prepareQueue(string qName, Boolean deleteWhenExists) {
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(azureConnectionString);
            if (namespaceManager.QueueExists(qName))
            {
                if (deleteWhenExists)
                {
                    namespaceManager.DeleteQueue(qName);
                    namespaceManager.CreateQueue(qName);
                }
            }
            return namespaceManager.GetQueue(qName);
        }

        /// <summary>
        /// Read message from queue
        /// </summary>
        /// <param name="qName"></param>
        /// <returns></returns>
        public void readMessageFromBus(string qName) {
            var client = QueueClient.CreateFromConnectionString(azureConnectionString, qName, ReceiveMode.PeekLock);
            while (true)
            {
                BrokeredMessage message = client.Receive();
                if (message != null)
                {
                    try
                    {
                        Console.WriteLine("Body: " + message.GetBody<string>());
                        Console.WriteLine("MessageID: " + message.MessageId);
                        Console.WriteLine("Test Property: " +
                           message.Properties["TestProperty"]);

                        // Remove message from queue
                        message.Complete();
                    }
                    catch (Exception)
                    {
                        // Indicate a problem, unlock message in queue
                        message.Abandon();
                    }
                }
            }
        }

    }
}
