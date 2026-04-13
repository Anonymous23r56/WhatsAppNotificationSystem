using NotificationApi.DTOs;

namespace NotificationApi.Validators
{
    public static class BankTransactionValidator
    {
        public static (bool IsValid, List<string> Errors) Validate(BankTransactionDto dto)
        {
            var errors = new List<string>();

            if (dto.AccountId <= 0)
                errors.Add("A valid account ID is required.");

            if (dto.Amount <= 0)
                errors.Add("Amount must be greater than zero.");

            if (dto.Amount > 10_000_000)
                errors.Add("Amount exceeds maximum allowed transaction limit of ₦10,000,000.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                errors.Add("Transaction description is required.");

            if (dto.Description?.Length > 200)
                errors.Add("Description must not exceed 200 characters.");

            return (errors.Count == 0, errors);
        }
        public static (bool IsValid, List<string> Errors) ValidateTransfer(BankTransactionDto dto)
        {
            var (isValid, errors) = Validate(dto);

            if (dto.TargetAccountId == null || dto.TargetAccountId <= 0)
                errors.Add("A valid target account ID is required for transfers.");

            if (dto.TargetAccountId == dto.AccountId)
                errors.Add("Source and target accounts cannot be the same.");

            return (errors.Count == 0, errors);
        }

    }
}
