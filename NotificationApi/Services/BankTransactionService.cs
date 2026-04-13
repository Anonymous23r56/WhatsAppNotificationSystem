using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models;
using NotificationApi.Services.Interfaces;

namespace NotificationApi.Services 
{
    public class BankTransactionService : IBankTransactionService
    {
        private readonly AppDbContext _db;
        private readonly IKafkaProducerService _kafka;

        public BankTransactionService(AppDbContext db, IKafkaProducerService kafka)
        {
            _db = db;
            _kafka = kafka;
        }

        public async Task<Transaction> DepositAsync(int accountId,
            decimal amount, string description)
        {
            var account = await _db.Accounts.FindAsync(accountId)
                ?? throw new Exception("Account not found");

            var balanceBefore = account.Balance;
            account.Balance += amount;

            var transaction = new Transaction
            {
                AccountId = accountId,
                Type = "Credit",
                Amount = amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = account.Balance,
                Description = description,
                TransactionRef = GenerateRef(),
                Status = "Success",
                CreatedAt = DateTime.UtcNow
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            await PublishNotificationAsync(account, transaction);
            return transaction;
        }

        public async Task<Transaction> WithdrawAsync(int accountId,
            decimal amount, string description)
        {
            var account = await _db.Accounts.FindAsync(accountId)
                ?? throw new Exception("Account not found");

            if (account.Balance < amount)
                throw new Exception("Insufficient funds");

            var balanceBefore = account.Balance;
            account.Balance -= amount;

            var transaction = new Transaction
            {
                AccountId = accountId,
                Type = "Debit",
                Amount = amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = account.Balance,
                Description = description,
                TransactionRef = GenerateRef(),
                Status = "Success",
                CreatedAt = DateTime.UtcNow
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            await PublishNotificationAsync(account, transaction);
            return transaction;
        }

        public async Task<Transaction> TransferAsync(int fromAccountId,
            int toAccountId, decimal amount, string description)
        {
            var fromAccount = await _db.Accounts.FindAsync(fromAccountId)
                ?? throw new Exception("Source account not found");

            var toAccount = await _db.Accounts.FindAsync(toAccountId)
                ?? throw new Exception("Target account not found");

            if (fromAccount.Balance < amount)
                throw new Exception("Insufficient funds");

            var balanceBefore = fromAccount.Balance;
            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            var txRef = GenerateRef();

            var transaction = new Transaction
            {
                AccountId = fromAccountId,
                Type = "Transfer",
                Amount = amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = fromAccount.Balance,
                Description = $"{description} → {toAccount.FullName}",
                TransactionRef = txRef,
                Status = "Success",
                CreatedAt = DateTime.UtcNow
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            await PublishNotificationAsync(fromAccount, transaction);
            return transaction;
        }

        public async Task<List<Transaction>> GetHistoryAsync(int accountId)
        {
            return await _db.Transactions
                .Include(t => t.Account)
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        private async Task PublishNotificationAsync(Account account,
            Transaction transaction)
        {
            await _kafka.PublishAsync(new Shared.Events.TransactionEvent
            {
                TransactionRef = transaction.TransactionRef,
                AccountNumber = account.AccountNumber,
                PhoneNumber = account.PhoneNumber,
                Amount = transaction.Amount,
                Balance = transaction.BalanceAfter,
                Type = transaction.Type,
                Description = transaction.Description,
                Timestamp = transaction.CreatedAt
            });
        }

        private string GenerateRef()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
        }
    }
}