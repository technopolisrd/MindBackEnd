using System.Runtime.Serialization;

namespace Mind.Entity.Tables.DTO;

#nullable disable

public class DropDownDTO
{
    [DataMember]
    public string Code { get; set; }

    [DataMember]
    public string Name { get; set; }
}
