
namespace Common.Core.Contracts.Common
{
    public interface IBusinessEngine
    {
    }

    public interface IBusinessEngine<T> : IBusinessEngine
    where T : class, new()
    {
    }
}
