using System.Configuration;
using DataAccess.Company.Requests;
using DataAccess.Mapping;
using Domain.Company;
using Domain.Company.Interfaces;

namespace DataAccess.Company.Repositories
{
    public class PaymentRepository : IPaymentRepository 
    {
        private readonly IObjectAdapter<NewPayment, AddPaymentRequest> _adapter;

        public PaymentRepository(IObjectAdapter<NewPayment, AddPaymentRequest> adapter)
        {
            _adapter = adapter;
        }

        public bool Add(NewPayment payment)
        {
            var request = _adapter.Adapt(payment);

            /* Here, magical things happen and you submit your request
             * to your web service, ORM, stored procedure, or whatever else
             * your universe uses to actually do data work. For the purposes
             * of this example, we're using a crappy app setting. You can use 
             * the ResultDetails class with the associated Error/Warning info
             * objects to play with this a bit more if you want to mimic your 
             * own environment a bit better. */

            return bool.Parse(ConfigurationManager.AppSettings["RepositoryCallWillFail"]);
        }
    }
}
