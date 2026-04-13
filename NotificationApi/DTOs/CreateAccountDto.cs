namespace NotificationApi.DTOs
{
    public class CreateAccountDto
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public decimal InitialDeposit { get; set; } = 0;
    }
}