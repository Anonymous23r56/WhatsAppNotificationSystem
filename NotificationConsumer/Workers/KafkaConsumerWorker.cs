using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using NotificationConsumer.Services;
using Shared.Data;
using Shared.Events;
using Shared.Models;
using System.Text.Json;
using NotificationConsumer.Services.Interfaces;

namespace NotificationConsumer.Workers
{
    public class KafkaConsumerWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ITwilioWhatsAppService _twilioService;
        private readonly ILogger<KafkaConsumerWorker> _logger;
        private readonly IConfiguration _config;

        public KafkaConsumerWorker(
            IServiceScopeFactory scopeFactory,
            ITwilioWhatsAppService twilioService,
            ILogger<KafkaConsumerWorker> logger,
            IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _twilioService = twilioService;
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("KafkaConsumerWorker started. Listening on topic: {Topic}",
                _config["Kafka:Topic"]);

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _config["Kafka:BootstrapServers"],
                GroupId = _config["Kafka:ConsumerGroup"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe(_config["Kafka:Topic"]);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(5));
                    if (consumeResult == null) continue;

                    _logger.LogInformation("Message received from Kafka. Key: {Key}",
                        consumeResult.Message.Key);

                    var transactionEvent = JsonSerializer.Deserialize<TransactionEvent>(
                        consumeResult.Message.Value);

                    if (transactionEvent == null)
                    {
                        _logger.LogWarning("Failed to deserialize Kafka message.");
                        continue;
                    }

                    await ProcessEventAsync(transactionEvent);
                    consumer.Commit(consumeResult);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error processing Kafka message. Error: {Message}. Inner:{Inner}",
                        ex.Message,
                        ex.InnerException?.Message??"None");
                    await Task.Delay(2000, stoppingToken);
                }
            }

            consumer.Close();
            _logger.LogInformation("KafkaConsumerWorker stopped.");
        }

        private async Task ProcessEventAsync(TransactionEvent transactionEvent)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var setupService = new NotificationSetupService(db);
            var logService = new NotificationLogService(db);

            // Get template from tbl_Notification_Setup
            var setup = await setupService.GetSetupAsync(transactionEvent.Type);
            if (setup == null)
            {
                _logger.LogWarning("No active setup found for type: {Type}",
                    transactionEvent.Type);
                return;
            }

            // Format message using template
            var values = new Dictionary<string, string>
            {
                { "Amount", transactionEvent.Amount.ToString("N2") },
                { "AccountNumber", transactionEvent.AccountNumber },
                { "Description", transactionEvent.Description },
                { "Balance", transactionEvent.Balance.ToString("N2") },
                { "TransactionRef", transactionEvent.TransactionRef },
                { "Date", transactionEvent.Timestamp.ToString("dd MMM yyyy, hh:mm tt") }
            };

            var formattedMessage = setupService.FormatMessage(setup.Template, values);

            // Save to tbl_Notifications as Pending
            var notification = new Notification
            {
                SetupId = setup.Id,
                PhoneNumber = transactionEvent.PhoneNumber,
                Message = formattedMessage,
                Status = "Pending",
                TransactionRef = transactionEvent.TransactionRef,
                Amount = transactionEvent.Amount,
                AccountNumber = transactionEvent.AccountNumber,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await logService.CreateAsync(notification);
            _logger.LogInformation("Notification saved to DB. Id: {Id}", saved.Id);

            // Send via Twilio WhatsApp
            var success = await _twilioService.SendAsync(
                transactionEvent.PhoneNumber,
                formattedMessage
            );

            // Update status
            await logService.UpdateStatusAsync(
                saved.Id,
                success ? "Sent" : "Failed",
                success ? null : "Twilio delivery failed"
            );

            _logger.LogInformation(
                "Notification {Id} status updated to {Status}",
                saved.Id, success ? "Sent" : "Failed");
        }
    }
}
