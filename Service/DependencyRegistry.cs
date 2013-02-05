using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Company.Enums;
using StructureMap.Configuration.DSL;

namespace Service
{
    public class DependencyRegistry : Registry
    {

        public DependencyRegistry()
        {

        }

        public DependencyRegistry(Injection registryType)
        {
            switch (registryType)
            {
                case Injection.DataAccess:
                    StructureMap.ObjectFactory.Configure(x => x.AddRegistry(new DataAccess.DependencyRegistry()));
                    break;
                case Injection.ServiceLayer:
                    StructureMap.ObjectFactory.Configure(x => x.AddRegistry(new DependencyRegistry()));
                    break;
            }

        }
    }
}
