using System;
using System.Collections.Generic;
using System.Text;
using Shared.Data;
using Shared.Models;

namespace NotificationConsumer.Services
{
    public class NotificationLogService
    {
        private readonly AppDbContext _db;

        public NotificationLogService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();
            return notification;
        }

        public async Task UpdateStatusAsync(int id, string status,
            string? errorMessage = null)
        {
            var notification = await _db.Notifications.FindAsync(id);
            if (notification == null) return;

            notification.Status = status;
            if (status == "Sent") notification.SentAt = DateTime.UtcNow;
            if (errorMessage != null) notification.ErrorMessage = errorMessage;

            await _db.SaveChangesAsync();
        }
    }
}
