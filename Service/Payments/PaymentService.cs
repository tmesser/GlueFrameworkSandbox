using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Company;
using Domain.Company.Interfaces;

namespace Service.Payments
{
    public class PaymentService
    {
        private readonly IPaymentRepository _repository;

        public PaymentService(IPaymentRepository repository)
        {
            _repository = repository;
        }

        public void Add(NewPayment payment)
        {
            if (_repository.Add(payment))
            {
                // Hooray, everything is fine!
            }
            else
            {
                // Oh no, everything is not fine!
            }
        }
    }
}
