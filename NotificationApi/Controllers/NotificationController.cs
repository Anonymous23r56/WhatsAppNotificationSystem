using Microsoft.AspNetCore.Mvc;
using NotificationApi.DTOs;
using NotificationApi.Services.Interfaces;
using NotificationApi.Validators;
using Shared.Events;

namespace NotificationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IKafkaProducerService _kafkaProducer;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(IKafkaProducerService kafkaProducer,
            ILogger<NotificationController> logger)
        {
            _kafkaProducer = kafkaProducer;
            _logger = logger;
        }

        [HttpPost("trigger")]
        public async Task<IActionResult> TriggerNotification(
            [FromBody] TransactionRequestDto dto)
        {
            var transactionEvent = new TransactionEvent
            {
                TransactionRef = dto.TransactionRef,
                AccountNumber = dto.AccountNumber,
                PhoneNumber = dto.PhoneNumber,
                Amount = dto.Amount,
                Balance = dto.Balance,
                Type = dto.Type,
                Description = dto.Description,
                Timestamp = DateTime.UtcNow
            };

            var (isValid, errors) = TransactionEventValidator.Validate(transactionEvent);
            if (!isValid)
            {
                _logger.LogWarning("TransactionEvent validation failed: {Errors}",
                    string.Join(", ", errors));
                return BadRequest(new { errors });
            }

            try
            {
                await _kafkaProducer.PublishAsync(transactionEvent);
                _logger.LogInformation("Notification triggered. Ref: {Ref}",
                    dto.TransactionRef);

                return Ok(new
                {
                    status = "queued",
                    transactionRef = dto.TransactionRef,
                    message = "Event published to Kafka successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to trigger notification. Error: {Error}",
                    ex.Message);
                return StatusCode(500, new
                {
                    status = "failed",
                    message = ex.Message
                });
            }
        }
    }
}