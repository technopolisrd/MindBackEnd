namespace Common.Core.Contracts.Common
{
    public interface IDeferrableEntity
    {
        bool Deferred { get; set; }
    }
}
