using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Data;
using Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace NotificationDashboard.Pages
{
    public class TransactionsModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;

        public TransactionsModel(AppDbContext db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty] public int AccountId { get; set; }
        [BindProperty] public string Type { get; set; } = "Deposit";
        [BindProperty] public decimal Amount { get; set; }
        [BindProperty] public int? TargetAccountId { get; set; }
        [BindProperty] public string Description { get; set; } = string.Empty;

        public List<Account> Accounts { get; set; } = new();
        public List<TransactionViewModel> RecentTransactions { get; set; } = new();
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            if (TempData["SuccessMessage"] is string success)
                SuccessMessage = success;

            if (TempData["ErrorMessage"] is string error)
                ErrorMessage = error;

            await LoadDataAsync();
        }
        //public async Task<IActionResult> OnPostAsync()
        //{
        //    try
        //    {
        //        var client = _httpClientFactory.CreateClient("NotificationApi");
        //        var endpoint = Type switch
        //        {
        //            "Deposit" => "/api/BankTransactions/deposit",
        //            "Withdraw" => "/api/BankTransactions/withdraw",
        //            "Transfer" => "/api/BankTransactions/transfer",
        //            _ => throw new Exception("Invalid transaction type")
        //        };

        //        var payload = new
        //        {
        //            accountId = AccountId,
        //            amount = Amount,
        //            description = Description,
        //            targetAccountId = TargetAccountId
        //        };

        //        var content = new StringContent(
        //            JsonSerializer.Serialize(payload),
        //            Encoding.UTF8, "application/json");

        //        var response = await client.PostAsync(endpoint, content);

        //        if (response.IsSuccessStatusCode)
        //            SuccessMessage = $"✅ {Type} of ₦{Amount:N2} was successful! WhatsApp notification has been sent.";
        //        else
        //            ErrorMessage = await response.Content.ReadAsStringAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessage = ex.Message;
        //    }

        //    await LoadDataAsync();
        //    return Page();
        //}
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("NotificationApi");
                var endpoint = Type switch
                {
                    "Deposit" => "/api/BankTransactions/deposit",
                    "Withdraw" => "/api/BankTransactions/withdraw",
                    "Transfer" => "/api/BankTransactions/transfer",
                    _ => throw new Exception("Invalid transaction type")
                };

                var payload = new
                {
                    accountId = AccountId,
                    amount = Amount,
                    description = Description,
                    targetAccountId = TargetAccountId
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"✅ {Type} of ₦{Amount:N2} was successful!. Thanks for trusting us 😊";
                    return RedirectToPage("/Transactions");
                }
                else
                {
                    TempData["ErrorMessage"] = await response.Content.ReadAsStringAsync();
                    return RedirectToPage("/Transactions");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/Transactions");
            }
        }

        private async Task LoadDataAsync()
        {
            Accounts = await _db.Accounts
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            RecentTransactions = await _db.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .Select(t => new TransactionViewModel
                {
                    Id = t.Id,
                    Type = t.Type,
                    Amount = t.Amount,
                    BalanceAfter = t.BalanceAfter,
                    Description = t.Description,
                    TransactionRef = t.TransactionRef,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    AccountNumber = t.Account.AccountNumber,
                    AccountName = t.Account.FullName
                })
                .ToListAsync();
        }
    }

    public class TransactionViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Description { get; set; } = string.Empty;
        public string TransactionRef { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
    }
}