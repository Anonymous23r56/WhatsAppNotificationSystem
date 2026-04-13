using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Data;
using Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace NotificationDashboard.Pages
{
    public class TemplatesModel : PageModel
    {
        private readonly AppDbContext _db;
        public TemplatesModel(AppDbContext db) => _db = db;

        public List<NotificationSetup> Templates { get; set; } = new();

        public async Task OnGetAsync()
        {
            Templates = await _db.NotificationSetups
                .OrderBy(t => t.Id)
                .ToListAsync();
        }
    }
}