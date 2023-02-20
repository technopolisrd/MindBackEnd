
using Common.Core.Contracts.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Mind.Entity.SecurityAccount;

namespace Mind.Entity.Tables;

#nullable disable

[DataContract]
[Table("LOG_Movements")]
public class LogMovements
{

    #region Properties

    [DataMember]
    [Column("MovementId")]
    [Display(Name = "Movement ID")]
    public int ID { get; set; }

    [DataMember]
    [Display(Name = "Customer")]
    public int CustomerID { get; set; }

    [DataMember]
    [Display(Name = "User")]
    public int AccountID { get; set; }

    [DataMember]
    [Required]
    [Display(Name = "Start Date")]
    [Column(TypeName = "datetime2")]
    public DateTime StartDate { get; set; }

    [DataMember]
    [Required]
    [Display(Name = "End Date")]
    [Column(TypeName = "datetime2")]
    public DateTime EndDate { get; set; }

    [DataMember]
    [Display(Name = "Action")]
    public string Action { get; set; }

    #endregion

    #region Navigation

    public Customer customer { get; set; }

    #endregion

}
