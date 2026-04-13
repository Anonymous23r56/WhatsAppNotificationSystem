using System.Globalization;

namespace NotificationApi.DTOs
{
    public class TransactionRequestDto
    {
        public string TransactionRef { get; set; } = string.Empty;
        public string AccountNumber { get; set;} = string.Empty; 
        public string PhoneNumber { get; set;} = string.Empty;
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
