using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Data;
using Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace NotificationDashboard.Pages
{
    public class AccountsModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountsModel(AppDbContext db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty] public string FullName { get; set; } = string.Empty;
        [BindProperty] public string PhoneNumber { get; set; } = string.Empty;
        [BindProperty] public decimal InitialDeposit { get; set; }

        public List<Account> Accounts { get; set; } = new();
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            Accounts = await _db.Accounts
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("NotificationApi");
                var payload = new
                {
                    fullName = FullName,
                    phoneNumber = PhoneNumber,
                    initialDeposit = InitialDeposit
                };
                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/Accounts", content);
                if (response.IsSuccessStatusCode)
                    SuccessMessage = $"Account created successfully for {FullName}!";
                else
                    ErrorMessage = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            Accounts = await _db.Accounts.OrderByDescending(a => a.CreatedAt).ToListAsync();
            return Page();
        }
    }
}