namespace DataAccess.Company
{
    public class PaymentInfo
    {
        public DbDate[] SpecialPaymentDates { get; set; }
        public int SerialNumber { get; set; }
        public DbDate NextPaymentDate { get; set; }
        public string SourceAccount { get; set; }
        public Enums.Currency CurrencyType { get; set; }
    }
}
