using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationConsumer.Services.Interfaces
{
    public interface INotificationSetupService
    {
        Task<NotificationSetup?> GetSetupAsync(string notificationType);
        string FormatMessage(string template, Dictionary<string, string> values);
    }
}
