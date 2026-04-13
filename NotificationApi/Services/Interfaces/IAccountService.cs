using System;
using Shared.Models;
using NotificationApi.DTOs;

namespace NotificationApi.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(string fullname, string phoneNumber, decimal initialDeposit);
        Task<List<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
    }
}
