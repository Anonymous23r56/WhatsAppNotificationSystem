using Confluent.Kafka;
using Microsoft.AspNetCore.Identity;
using Shared.Events;
using System.Text.Json;
using NotificationApi.Services.Interfaces;

namespace NotificationApi.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;
        private readonly ILogger<KafkaProducerService> _logger;

        public KafkaProducerService(IConfiguration config,
            ILogger<KafkaProducerService> logger)
        {
            _logger = logger;
            _topic = config["Kafka:Topic"]!;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

        public async Task PublishAsync(TransactionEvent transactionEvent)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key = transactionEvent.TransactionRef,
                    Value = JsonSerializer.Serialize(transactionEvent)
                };

                var result = await _producer.ProduceAsync(_topic, message);

                _logger.LogInformation(
                    "Event published to Kafka. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                    result.Topic, result.Partition, result.Offset);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to publish event to Kafka. Error: {Error}",
                    ex.Message);
                throw;
            }
        }
    }
}