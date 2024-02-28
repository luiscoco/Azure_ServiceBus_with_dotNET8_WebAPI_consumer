using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace ServiceBusReceiverApi.Controllers
{
    public class MessageDto
    {
        public string? Body { get; set; }
        public string? Priority { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ServiceBusController : ControllerBase
    {
        private static string connectionString = "Endpoint=sb://myservicebus1974.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=c/7ve5kw9QuPqM8YSUWQvNTrjM+y5hkmp+ASbE85qY4=";
        private static string topicName = "mytopic";
        private static string subscriptionName = "mysubscription";
        private static ServiceBusClient client;
        private static ServiceBusProcessor processor;
        private static ConcurrentQueue<MessageDto> receivedMessages = new ConcurrentQueue<MessageDto>();

        static ServiceBusController()
        {
            client = new ServiceBusClient(connectionString);

            var processorOptions = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = true
            };

            processor = client.CreateProcessor(topicName, subscriptionName, processorOptions);
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler; // Add error handler
        }

        public static async Task StartMessageProcessing()
        {
            await processor.StartProcessingAsync();
        }

        [HttpGet("receive")]
        public ActionResult<IEnumerable<MessageDto>> ReceiveMessages(string? priority = null)
        {
            if (string.IsNullOrEmpty(priority))
            {
                return receivedMessages.ToList();
            }
            else
            {
                return receivedMessages.Where(m => m.Priority == priority).ToList();
            }
        }

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            string priority = args.Message.ApplicationProperties["priority"]?.ToString() ?? "normal";
            Console.WriteLine($"Received message: {body}, Priority: {priority}");

            receivedMessages.Enqueue(new MessageDto { Body = body, Priority = priority });

            await Task.CompletedTask;
        }

        static async Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error source: {args.ErrorSource}");
            Console.WriteLine($"Fully qualified namespace: {args.FullyQualifiedNamespace}");
            Console.WriteLine($"Entity path: {args.EntityPath}");
            Console.WriteLine($"Exception: {args.Exception.Message}");

            await Task.CompletedTask;
        }
    }
}
