using Microsoft.AspNetCore.Mvc;
using NotificationApi.DTOs;
using NotificationApi.Services.Interfaces;
using NotificationApi.Validators;

namespace NotificationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService,
            ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
        {
            var (isValid, errors) = CreateAccountValidator.Validate(dto);
            if (!isValid)
            {
                _logger.LogWarning("CreateAccount validation failed: {Errors}",
                    string.Join(", ", errors));
                return BadRequest(new { errors });
            }

            try
            {
                var account = await _accountService.CreateAccountAsync(
                    dto.FullName, dto.PhoneNumber, dto.InitialDeposit);
                _logger.LogInformation("Account created: {AccountNumber}",
                    account.AccountNumber);
                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateAccount failed: {Error}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _accountService.GetAllAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { error = "Invalid account ID." });

            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
                return NotFound(new { error = "Account not found." });

            return Ok(account);
        }
    }
}