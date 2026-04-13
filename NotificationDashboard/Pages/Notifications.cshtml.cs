using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models;

namespace NotificationDashboard.Pages
{
    public class NotificationsModel : PageModel
    {
        private readonly AppDbContext _db;
        public NotificationsModel(AppDbContext db) => _db = db;

        [BindProperty(SupportsGet = true)] public string? Filter { get; set; }
        public List<Notification> Notifications { get; set; } = new();
        public int TotalSent { get; set; }
        public int TotalPending { get; set; }
        public int TotalFailed { get; set; }

        public async Task OnGetAsync()
        {
            TotalSent = await _db.Notifications.CountAsync(n => n.Status == "Sent");
            TotalPending = await _db.Notifications.CountAsync(n => n.Status == "Pending");
            TotalFailed = await _db.Notifications.CountAsync(n => n.Status == "Failed");

            var query = _db.Notifications.AsQueryable();

            if (!string.IsNullOrEmpty(Filter) && Filter != "All")
                query = query.Where(n => n.Status == Filter);

            Notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}