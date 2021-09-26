﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RMQConnection = RabbitMQ.Client.IConnection;
using RMQConsumerChannel = RabbitMQ.Client.IModel;
using RMQConnectionFactory = RabbitMQ.Client.ConnectionFactory;
using RMQConsumerChannelExtensions = RabbitMQ.Client.IModelExensions;
using System;
using System.Collections.Generic;
using System.Threading;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Events;
using WorldManagementApi.Messaging.Configurations;
using WorldManagementApi.Messaging.Interfaces;
using WorldManagementApi.Models;
using Microsoft.Extensions.Configuration;
using WorldManagementApi.Services;

namespace WorldManagementApi.Messaging.Services
{
    /// <summary>
    /// Listener for additions, deletions, and changes to Mission objects 
    /// made by the Sample service. 
    /// </summary>
    public class KarmaListenerService : IWorldListenerService, IDisposable
    {
        private RabbitMQSettings settings;
        private ILogger<KarmaListenerService> logger;
        private IServiceScopeFactory serviceScopeFactory;
        private IRabbitMqService mRabbitMqService;
        private readonly IConfiguration _configuration;

        private RMQConnection rmqConnection;
        private RMQConsumerChannel consumerChannel;
        private Dictionary<string, CustomBasicConsumer> consumers;
        private const string ConsumerQueueKeyFormat = "World_receiving_from_{0}";

        /// <summary>
        /// This is a background service that processess messages received from Mosanc
        /// via RabbitMQ. This will interpret messages and update the database accordingly.
        /// </summary>
        /// <param name="config">
        /// Application configuration values
        /// </param>
        /// <param name="logger">
        /// Logger for error and info loggins
        /// </param>
        /// <param name="serviceScopeFactory">
        /// Service Scope for DI
        /// </param>
        public KarmaListenerService(
          IOptions<RabbitMQSettings> config,
          ILogger<KarmaListenerService> logger,
          IServiceScopeFactory serviceScopeFactory,
          IRabbitMqService rabbitMqService,
              IConfiguration configuration)
        {
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
            this.mRabbitMqService = rabbitMqService;
            this._configuration = configuration;
            settings = config.Value;
            consumers = new Dictionary<string, CustomBasicConsumer>();

            logger.LogInformation("Starting World Listener Service...");

            BuildConnection();

            if (rmqConnection != null && rmqConnection.IsOpen)
            {
                BindMessageConsumers();
            }
            else
            {
                logger.LogWarning("Connection to RabbitMQ failed in WorldListenerService. Messages will not be consumed. Check settings and restart this pod.");
            }
        }

        private void BuildConnection()
        {
            var factory = new RMQConnectionFactory()
            {
                HostName = settings.Host,
                UserName = settings.UserName,
                Password = settings.Password
            };
            logger.LogInformation("Created connection factory with:");
            logger.LogInformation($"Hostname: {settings.Host}");
            logger.LogInformation($"Username: {settings.UserName}");

            bool success = false;
            int tries = 0;
            while (!success && tries <= settings.NumberOfConnectionRetries)
            {
                try
                {
                    rmqConnection = factory.CreateConnection();
                    logger.LogInformation("RabbitMQ Connection successfully opened");
                    success = true;
                }
                catch (BrokerUnreachableException)
                {
                    logger.LogWarning($"Unable to reach RabbitMQ server. Retrying in {settings.ConnectionRetryInterval} seconds...");
                    tries++;
                    Thread.Sleep(settings.ConnectionRetryInterval * 1000);
                }
                catch (AuthenticationFailureException)
                {
                    logger.LogError("Username and/or Password in RabbitMQSettings is incorrect. Check configuration values and restart this pod");
                    break;
                }
            }
        }

        private void BindMessageConsumers()
        {
            consumerChannel = rmqConnection.CreateModel();
            logger.LogInformation("RabbitMQ Channel successfully opened");

            // Declare a home exchange for outgoing messages
            consumerChannel.ExchangeDeclare(settings.HomeExchange, RabbitMQ.Client.ExchangeType.Topic, true, false, null);
            logger.LogInformation("Declared exchange {0}", settings.HomeExchange);

            // Declare exchanges and declare queues for anything we should be listening for.
            foreach (string topic in settings.ListeningTopics)
            {
                if (topic.Length == 0)
                {
                    continue;
                }

                string[] parts = topic.Split('.');
                if (parts[0] == "*" || parts[0] == "#")
                {
                    logger.LogWarning($"An exchange was not specified for topic {topic}. Skipping topic.");
                    continue;
                }

                AddListener(parts[0], topic);
            }
        }

        private void AddListener(string exchangeName, string topicKey)
        {
            var queueName = string.Format(ConsumerQueueKeyFormat, exchangeName);
            var consumerKey = queueName;

            consumerChannel.ExchangeDeclare(
              exchange: exchangeName,
              type: RabbitMQ.Client.ExchangeType.Topic,
              durable: true,
              autoDelete: false,
              arguments: null);

            CustomBasicConsumer consumer;
            if (!consumers.TryGetValue(consumerKey, out consumer))
            {
                // the key/value pair in arguments causes the queue to be of type quorum instead of classic, which is preferred
                var queueDeclareArgs = new Dictionary<string, object> { { "x-queue-type", "quorum" } };

                // also changing the auto-delete param to false so that data will not be lost should all consumers become disconnected
                consumerChannel.QueueDeclare(queueName, true, false, false, queueDeclareArgs);
                consumerChannel.QueueBind(queueName, exchangeName, topicKey, null);

                consumer = new CustomBasicConsumer(consumerChannel);
                consumers.Add(consumerKey, consumer);

                consumer.Received += (object sender, BasicDeliverEventArgs args) =>
                {
                    var payload = System.Text.Encoding.UTF8.GetString(args.Body.ToArray());

                    ProcessInboundMessage(args.RoutingKey, payload);

                    consumerChannel.BasicAck(args.DeliveryTag, false);
                };

                var consumerTag = RMQConsumerChannelExtensions.BasicConsume(consumerChannel, queueName, false, consumer);
                consumer.ConsumerTag = consumerTag;
                consumer.TopicCounter++;
            }
            else
            {
                consumerChannel.QueueBind(queueName, exchangeName, topicKey, null);
            }
        }

        private async void ProcessInboundMessage(string topic, string payload)
        {
            if (topic == "people_exchange_main.person.created")
            {
                Console.WriteLine("Message Recieved " + topic);
                var person = JsonConvert.DeserializeObject<Person>(payload);
                var peopleService = new PeopleService(_configuration, mRabbitMqService);
                var addedPerson = peopleService.Add(person);
            }

            if (topic == "people_exchange_main.person.updated")
            {
                Console.WriteLine("Message Recieved " + topic);
                var person = JsonConvert.DeserializeObject<Person>(payload);
                var peopleService = new PeopleService(_configuration, mRabbitMqService);
                var updatedPerson = peopleService.Update(person);
            }


        }

        /// <summary>
        /// Cleans up any unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            logger.LogInformation("Closing RabbitMQ Connection for WorldListenerService...");

            foreach (CustomBasicConsumer consumer in consumers.Values)
            {
                if (consumer.Model.IsOpen)
                {
                    consumer.Model.Close();
                }
            }

            if (consumerChannel != null && consumerChannel.IsOpen)
            {
                consumerChannel.Close();
            }

            if ((rmqConnection != null) && (rmqConnection.IsOpen))
            {
                rmqConnection.Close();
            }

            logger.LogInformation("RabbitMQ Connection Closed for WorldListenerService");
        }

        private class CustomBasicConsumer : EventingBasicConsumer
        {
            public CustomBasicConsumer(RMQConsumerChannel model)
              : base(model)
            {
                TopicCounter = 0;
            }

            public uint TopicCounter { get; set; }

            /// <summary>
            /// This property was removed from the base class in .Net 3.1.
            /// I have added it here to capture the functionality that 
            /// it seemed to be supporting.
            /// </summary>
            public string ConsumerTag { get; set; }
        }
    }
}
