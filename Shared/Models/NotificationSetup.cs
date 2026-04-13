using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class NotificationSetup
    {
        public int Id { get; set; }
        public string NotificationType { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string Channel { get; set; } = "WhatsApp";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //Navigation
        public ICollection<Notification> Notifications { get; set; }
        = new List<Notification>();
    }
}
