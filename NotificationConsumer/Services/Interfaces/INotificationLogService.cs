using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationConsumer.Services.Interfaces
{
    public interface INotificationLogService
    {
        Task<Notification> CreateAsync(Notification notification);
        Task UpdateStatusAsync(int id, string status, string? errorMessage = null);
    }
}
