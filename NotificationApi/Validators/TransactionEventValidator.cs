using Shared.Events;
using System;


namespace NotificationApi.Validators
{
    public static class TransactionEventValidator
    {
        private static readonly string[] ValidTypes = { "Credit", "Debit", "Transfer" };

        public static (bool IsValid, List<string> Errors) Validate(TransactionEvent evt)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(evt.TransactionRef))
                errors.Add("Transaction reference is required.");

            if (string.IsNullOrWhiteSpace(evt.AccountNumber))
                errors.Add("Account number is required.");

            if (string.IsNullOrWhiteSpace(evt.PhoneNumber))
                errors.Add("Phone number is required.");

            if (evt.PhoneNumber != null && !evt.PhoneNumber.StartsWith("whatsapp:+"))
                errors.Add("Phone number must start with 'whatsapp:+'.");

            if (evt.Amount <= 0)
                errors.Add("Amount must be greater than zero.");

            if (evt.Balance < 0)
                errors.Add("Balance cannot be negative.");

            if (string.IsNullOrWhiteSpace(evt.Type))
                errors.Add("Transaction type is required.");

            if (!string.IsNullOrWhiteSpace(evt.Type) &&
                !Array.Exists(ValidTypes, t => t == evt.Type))
                errors.Add($"Transaction type must be one of: {string.Join(", ", ValidTypes)}.");

            if (string.IsNullOrWhiteSpace(evt.Description))
                errors.Add("Transaction description is required.");

            if (evt.Timestamp == default)
                errors.Add("Transaction timestamp is required.");

            return (errors.Count == 0, errors);
        }
    }
}