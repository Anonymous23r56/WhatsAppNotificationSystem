using System;
using System.Collections.Generic;
using System.Text;
using NotificationConsumer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models;

namespace NotificationConsumer.Services
{
    public class NotificationSetupService : INotificationSetupService
    {
        private readonly AppDbContext _db;

        public NotificationSetupService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<NotificationSetup?> GetSetupAsync(string notificationType)
        {
            return await _db.NotificationSetups
                .Where(s => s.NotificationType == notificationType && s.IsActive)
                .FirstOrDefaultAsync();
        }

        public string FormatMessage(string template, Dictionary<string, string> values)
        {
            var message = template;
            foreach (var kvp in values)
            {
                message = message.Replace($"{{{kvp.Key}}}", kvp.Value);
            }
            return message.Replace("\\n", "\n");
        }
    }
}