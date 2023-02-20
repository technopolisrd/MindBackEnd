namespace Common.Core.Contracts.Common
{
    public interface IConcurrencyEntity
    {
        byte[] RowVersion { get; set; }
    }
}
