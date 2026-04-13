using Shared.Models;

namespace NotificationApi.Services.Interfaces
{
    public interface IBankTransactionService
    {
        Task<Transaction> DepositAsync(int accountId, decimal amount, string description);
        Task<Transaction> WithdrawAsync(int accountId, decimal amount, string description);
        Task<Transaction> TransferAsync(int fromAccountId, int toAccountId, decimal amount, string description);
        Task<List<Transaction>> GetHistoryAsync(int accountId);
    }
}
