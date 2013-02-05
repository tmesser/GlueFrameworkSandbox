namespace DataAccess.Company.Requests
{
    public class AddPaymentRequest
    {
        public Customer PayingCustomer { get; set; }
        public PaymentInfo Payment { get; set; }
    }
}

