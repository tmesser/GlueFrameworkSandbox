using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Company.Repositories;
using DataAccess.Company.Requests;
using DataAccess.Mapping;
using Domain.Company;
using Domain.Company.Interfaces;
using StructureMap.Configuration.DSL;

namespace DataAccess
{
    public class DependencyRegistry : Registry 
    {
        public DependencyRegistry()
        {
            if (!StructureMap.ObjectFactory.Container.Model.HasImplementationsFor<IPaymentRepository>())
                For<IPaymentRepository>().Use<PaymentRepository>();
            if (!StructureMap.ObjectFactory.Container.Model.HasImplementationsFor<IObjectAdapter<NewPayment, AddPaymentRequest>>())
                For<IObjectAdapter<NewPayment, AddPaymentRequest>>().Use<BaseAdapter<NewPayment,AddPaymentRequest>>();
        }

    
    }
}
