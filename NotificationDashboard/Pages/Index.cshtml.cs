using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models;

namespace NotificationDashboard.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        public IndexModel(AppDbContext db) => _db = db;

        public int TotalSent { get; set; }
        public int TotalPending { get; set; }
        public int TotalFailed { get; set; }
        public int TotalAccounts { get; set; }
        public string SuccessRate { get; set; } = "0%";
        public List<Notification> RecentNotifications { get; set; } = new();

        public async Task OnGetAsync()
        {
            TotalSent = await _db.Notifications.CountAsync(n => n.Status == "Sent");
            TotalPending = await _db.Notifications.CountAsync(n => n.Status == "Pending");
            TotalFailed = await _db.Notifications.CountAsync(n => n.Status == "Failed");
            TotalAccounts = await _db.Accounts.CountAsync();

            var total = TotalSent + TotalPending + TotalFailed;
            SuccessRate = total > 0 ? $"{Math.Round((double)TotalSent / total * 100)}%" : "0%";

            RecentNotifications = await _db.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .Select(n => new Notification
                {
                    Id = n.Id,
                    AccountNumber = n.AccountNumber,
                    TransactionRef = n.TransactionRef,
                    Amount = n.Amount,
                    Status = n.Status,
                    CreatedAt = n.CreatedAt,
                    PhoneNumber = n.PhoneNumber,
                    Message = n.Message
                })
                .ToListAsync();
                    }
    }
}