namespace Domain.Company
{
    public class NewPayment
    {
        public DomainCustomer PayingCustomer { get; set; }
        public DomainPaymentInfo PaymentInfo { get; set; }
    }
}
