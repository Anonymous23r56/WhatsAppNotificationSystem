using Microsoft.AspNetCore.Mvc;
using NotificationApi.DTOs;
using NotificationApi.Services.Interfaces;
using NotificationApi.Validators;

namespace NotificationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankTransactionsController : ControllerBase
    {
        private readonly IBankTransactionService _transactionService;
        private readonly ILogger<BankTransactionsController> _logger;

        public BankTransactionsController(IBankTransactionService transactionService,
            ILogger<BankTransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] BankTransactionDto dto)
        {
            var (isValid, errors) = BankTransactionValidator.Validate(dto);
            if (!isValid)
            {
                _logger.LogWarning("Deposit validation failed: {Errors}",
                    string.Join(", ", errors));
                return BadRequest(new { errors });
            }

            try
            {
                var result = await _transactionService.DepositAsync(
                    dto.AccountId, dto.Amount, dto.Description);
                _logger.LogInformation("Deposit successful. Ref: {Ref}",
                    result.TransactionRef);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deposit failed: {Error}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] BankTransactionDto dto)
        {
            var (isValid, errors) = BankTransactionValidator.Validate(dto);
            if (!isValid)
            {
                _logger.LogWarning("Withdraw validation failed: {Errors}",
                    string.Join(", ", errors));
                return BadRequest(new { errors });
            }

            try
            {
                var result = await _transactionService.WithdrawAsync(
                    dto.AccountId, dto.Amount, dto.Description);
                _logger.LogInformation("Withdrawal successful. Ref: {Ref}",
                    result.TransactionRef);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Withdraw failed: {Error}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] BankTransactionDto dto)
        {
            var (isValid, errors) = BankTransactionValidator.ValidateTransfer(dto);
            if (!isValid)
            {
                _logger.LogWarning("Transfer validation failed: {Errors}",
                    string.Join(", ", errors));
                return BadRequest(new { errors });
            }

            try
            {
                var result = await _transactionService.TransferAsync(
                    dto.AccountId, dto.TargetAccountId!.Value,
                    dto.Amount, dto.Description);
                _logger.LogInformation("Transfer successful. Ref: {Ref}",
                    result.TransactionRef);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Transfer failed: {Error}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetHistory(int accountId)
        {
            if (accountId <= 0)
                return BadRequest(new { error = "Invalid account ID." });

            var history = await _transactionService.GetHistoryAsync(accountId);
            return Ok(history);
        }
    }
}