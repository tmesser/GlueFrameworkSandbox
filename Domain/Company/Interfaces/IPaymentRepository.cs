namespace Domain.Company.Interfaces
{
    public interface IPaymentRepository
    {
        /// <summary>
        /// Adds the specified payment.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <returns>Success/Failure boolean</returns>
        bool Add(NewPayment payment);
    }
}
