# How to create a .NET8 WebAPI for receiving messages from Azure ServiceBus

See the source code for this demo in this github repo: https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer

## 1. Create Azure ServiceBus (Topic and Subscrition)

We first log in to Azure Portal and search for Azure Service Bus 

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/c1083a36-37ed-41cd-b338-05b79338d256)

We create a new Azure Service Bus 

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/c55dfe80-c170-4a11-abd5-64fba5d3d038)

We input the required data: Subscription, ResourceGroup, Namespace, location and pricing tier

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/ceb2546c-a073-41c7-8ec5-0f29e59766fb)

We verify the new Azure Service Bus

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/d5d306e4-cea0-4898-a9e5-9b0ebb6d9eca)

We get the connection string

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/d540d906-ce3b-4d5d-b984-563a1895654b)

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/8c077842-6b05-46e2-a03e-de04f8bd1dcf)

This is the connection string:

```
Endpoint=sb://myservicebus1974.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=c/7ve5kw9QuPqM8YSUWQvNTrjM+y5hkmp+ASbE85qY4=
```

We have to create a new topic

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/4042c8cc-f5f3-4e0e-9dfc-139722d6297d)

We also have to create a new subscription

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/5da3f4ef-4617-4b29-9b8b-036ddd0e13e1)

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/6eabacda-7346-4ced-b66c-6fcb8c5815c7)

## 2. Create a .NET8 WebAPI with VSCode

Creating a .NET 8 Web API using Visual Studio Code (VSCode) and the .NET CLI is a straightforward process

This guide assumes you have .NET 8 SDK, VSCode, and the C# extension for VSCode installed. If not, you'll need to install these first

**Step 1: Install .NET 8 SDK**

Ensure you have the .NET 8 SDK installed on your machine: https://dotnet.microsoft.com/es-es/download/dotnet/8.0

You can check your installed .NET versions by opening a terminal and running:

```
dotnet --list-sdks
```

If you don't have .NET 8 SDK installed, download and install it from the official .NET download page

**Step 2: Create a New Web API Project**

Open a terminal or command prompt

Navigate to the directory where you want to create your new project

Run the following command to create a new Web API project:

```
dotnet new webapi -n ServiceBusReceiverApi
```

This command creates a new directory with the project name, sets up a basic Web API project structure, and restores any necessary packages

**Step 3: Open the Project in VSCode**

Once the project is created, you can open it in VSCode by navigating into the project directory and running:

```
code .
```

This command opens VSCode in the current directory, where . represents the current directory

## 3. Load project dependencies

We run this command to add the Azure Service Bus library

```
dotnet add package Azure.Messaging.ServiceBus
```

We also have to add the Swagger and OpenAPI libraries to access the API Docs

This is the csproj file including the project dependencies

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/2990d2e5-48bb-4239-b708-5b934664d5a5)

## 4. Create the project structure

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/475d1f86-cdb2-46c9-98c1-a16e31923f60)

## 5. Create the Controller

```csharp
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
```

## 6. Modify the application middleware(program.cs)

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using ServiceBusReceiverApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ServiceBusReceiverApi", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiceBusReceiverApi v1");
});

app.UseAuthorization();

app.MapControllers();

// Start message processing
ServiceBusController.StartMessageProcessing().Wait();

app.Run();
```

## 7. Run and Test the application

We execute this command to run the application

```
dotnet run
```

We receive in the VSCode console the messages from Azure Service Bus

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/59249bd1-8e8f-4308-8d54-48391be75043)

We can also see the messages received in Swagger: http://localhost:5221/swagger/index.html

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/1f5606a8-5cef-401b-8de5-2be07e2926b7)
