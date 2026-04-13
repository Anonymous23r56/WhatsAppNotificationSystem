using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Shared.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int SetupId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string TransactionRef { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }
        public string? ErrorMessage { get; set; }

        //Navigation
        public NotificationSetup Setup { get; set; } = null;
    }
}
