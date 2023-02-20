using System;
using Common.Core.Contracts.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Core.Engine
{
    public class BusinessEngineFactory : IBusinessEngineFactory
    {
        private readonly IServiceProvider services;

        public BusinessEngineFactory(IServiceProvider services)
        {
            this.services = services;
        }

        public T GetBusinessEngine<T>() where T : IBusinessEngine
        {
            //Import instance of T from the DI container
            return services.GetService<T>();
        }
    }
}
