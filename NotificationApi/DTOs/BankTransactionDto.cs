namespace NotificationApi.DTOs
{
    public class BankTransactionDto
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? TargetAccountId { get; set; }
    }
}
