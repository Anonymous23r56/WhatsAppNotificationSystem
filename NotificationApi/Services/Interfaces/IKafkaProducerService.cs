using System;
using Shared.Events;
namespace NotificationApi.Services.Interfaces
{
    public interface IKafkaProducerService
    {
        Task PublishAsync(TransactionEvent transactionEvent);
    }
}
