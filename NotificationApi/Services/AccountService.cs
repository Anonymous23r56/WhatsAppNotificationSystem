using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models;
using NotificationApi.Services.Interfaces;

namespace NotificationApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _db;

        public AccountService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Account> CreateAccountAsync(
            string fullName,
            string phoneNumber,
            decimal initialDeposit)
        {
            var account = new Account
            {
                FullName = fullName,
                PhoneNumber = phoneNumber,
                AccountNumber = GenerateAccountNumber(),
                Balance = initialDeposit,
                CreatedAt = DateTime.UtcNow
            };

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();
            return account;
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _db.Accounts
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _db.Accounts.FindAsync(id);
        }

        private string GenerateAccountNumber()
        {
            var random = new Random();
            var number = random.Next(1000000, 9999999);
            return $"ACC-{number}";
        }
    }
}