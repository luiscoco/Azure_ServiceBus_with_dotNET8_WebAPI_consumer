# How to create a .NET8 WebAPI for receiving messages from Azure ServiceBus

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

## 3. Load project dependencies

We run this command to add the Azure Service Bus library

```
dotnet add package Azure.Messaging.ServiceBus
```

We also have to add the Swagger and OpenAPI libraries to access the API Docs

This is the csproj file including the project dependencies

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/2990d2e5-48bb-4239-b708-5b934664d5a5)

## 4. Create the project structure



## 5. Create the Controller

## 6. Modify the application middleware(program.cs)

## 7. Run and Test the application
