
namespace Common.Core.Contracts.Common
{
    public interface IBusinessEngineFactory
    {
        T GetBusinessEngine<T>() where T : IBusinessEngine;
    }
}
