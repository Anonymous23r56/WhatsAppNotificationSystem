using NotificationApi.DTOs;
using System;

namespace NotificationApi.Validators
{
    public static class CreateAccountValidator
    {
        public static (bool IsValid, List<string> Errors) Validate (CreateAccountDto dto)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.FullName))
                errors.Add("Full name is required.");
     
            if (dto.FullName?. Length > 100)
                errors.Add("Full name must not exceed 100 characters.");
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                errors.Add("Phone number is required.");
            if (!dto.PhoneNumber?.StartsWith("whatsapp:+") ?? true)
                errors.Add("Phone number must start with 'whatsapp:+' e.g. whatsapp:+2348012345678");
           if (dto.InitialDeposit < 0)
                errors.Add("Initial deposit cannot be negative.");
            return (errors.Count == 0, errors);
        }
    }
}
