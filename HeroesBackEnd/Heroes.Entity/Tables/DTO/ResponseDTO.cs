namespace Mind.Entity.Tables.DTO;

#nullable disable

public class ResponseDTO<TEntity>
{
    public string status { get; set; }
    public string message { get; set; }
    public TEntity data { get; set; }
}
