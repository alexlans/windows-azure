using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace _267Industries.Azure.ServiceBus
{
    /// <summary>
    /// Class is responsable for connecting to a service bus of Windows Azure
    /// </summary>
    public sealed class AzureServiceBus
    {
        private static string azureConnectionEndpoint; 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="azureEndpoint">Entire Windows Azure Endpoint string</param>
        public AzureServiceBus(AzureServiceBusEndpoint azureEndpoint)
        {
            if (azureEndpoint == null || string.IsNullOrEmpty(azureEndpoint.Endpoint)) 
                throw new Exception("Azure Endpoint is empty and is mandatory");
            azureConnectionEndpoint = azureEndpoint.Endpoint;
        }

        /// <summary>
        /// Send message to the service bus of Windows Azure
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        public void SendMessage(string queueName, string message) {
            //Check if queue does exists
            prepareQueue(queueName);
            //Create service bus connection
            QueueClient azureServiceBus = QueueClient.CreateFromConnectionString(azureConnectionEndpoint, queueName, ReceiveMode.PeekLock);
            azureServiceBus.OnMessage((msg) =>
            {
            }, new OnMessageOptions()
            {
                AutoComplete = true
                //MaxConcurrentCalls = 5
            });
            azureServiceBus.Send(new BrokeredMessage(message));
        }

        /// <summary>
        /// Delete a queue on the service bus when it exists
        /// </summary>
        public void DeleteQueue(string queueName) {
         NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(azureConnectionEndpoint);
         if (namespaceManager.QueueExists(queueName))
         {
             namespaceManager.DeleteQueue(queueName);
         }
        }

        #region Private Methods

        /// <summary>
        /// Check if queue exists within Azure Service Bus. If it does not exist, create it
        /// </summary>
        /// <param name="qName"></param>
        /// <returns></returns>
        private QueueDescription prepareQueue(string qName)
        {
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(azureConnectionEndpoint);
            if (namespaceManager.QueueExists(qName) == false)
            {
                namespaceManager.CreateQueue(qName);
            }
            return namespaceManager.GetQueue(qName);
        }

        #endregion

    }
}
