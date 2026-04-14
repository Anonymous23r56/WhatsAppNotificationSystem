using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Quartz;
using CronJob.Services;
using Shared.Data;
using CronJob.Services.Interfaces;


namespace CronJob.Jobs
{
    [DisallowConcurrentExecution]
    public class RetryFailedNotificationsJob : IJob
    {
        private readonly AppDbContext _db;
        private readonly ITwilioWhatsAppService _twilioService;
        private readonly ILogger<RetryFailedNotificationsJob> _logger;

        public RetryFailedNotificationsJob(
            AppDbContext db,
            ITwilioWhatsAppService twilioService,
            ILogger<RetryFailedNotificationsJob> logger)
        {
            _db = db;
            _twilioService = twilioService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("CronJob running at {Time}", DateTime.UtcNow);

            // Pick up Pending and Failed notifications
            var pendingNotifications = await _db.Notifications
                .Where(n => n.Status == "Pending" || n.Status == "Failed")
                .ToListAsync();

            if (!pendingNotifications.Any())
            {
                _logger.LogInformation("No pending or failed notifications found.");
                return;
            }

            _logger.LogInformation(
                "Found {Count} notification(s) to retry.",
                pendingNotifications.Count);

            foreach (var notification in pendingNotifications)
            {
                _logger.LogInformation(
                    "Retrying notification {Id} to {Phone}",
                    notification.Id, notification.PhoneNumber);

                var success = await _twilioService.SendAsync(
                    notification.PhoneNumber,
                    notification.Message
                );

                if (success)
                {
                    notification.Status = "Sent";
                    notification.SentAt = DateTime.UtcNow;
                    notification.ErrorMessage = null;
                    _logger.LogInformation(
                        "Notification {Id} retried successfully.", notification.Id);
                }
                else
                {
                    notification.Status = "Failed";
                    notification.ErrorMessage = "Retry failed via CronJob";
                    _logger.LogWarning(
                        "Notification {Id} retry failed.", notification.Id);
                }

                await _db.SaveChangesAsync();
            }
        }
    }
}
