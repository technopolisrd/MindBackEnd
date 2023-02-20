namespace Mind.Entity.Tables;

using Common.Core.Contracts.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Mind.Entity.SecurityAccount;

#nullable disable

[DataContract]
[Table("CODE_Team")]
public class Team : IAuditableEntity, IDeferrableEntity, IConcurrencyEntity
{

    #region Properties

    [DataMember]
    [Column("TeamId")]
    [Display(Name = "Team ID")]
    public int ID { get; set; }

    [DataMember]
    [StringLength(100, ErrorMessage = "Ha excedido la cantidad máxima de caracteres permitidos en este campo.")]
    [Display(Name = "Team Name")]
    public string TeamName { get; set; }

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

    #endregion

    #region Navigation

    public Customer customer { get; set; }

    #endregion

    #region Interface Implentations

    [DataMember]
    [Required]
    [Column(TypeName = "datetime2")]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    [Required]
    public string CreatedById { get; set; }

    [DataMember]
    [Column(TypeName = "datetime2")]
    public DateTime UpdatedDate { get; set; }

    [DataMember]
    public string UpdatedById { get; set; }

    [DataMember]
    [Required]
    [Column("DeleteFlag")]
    public bool Deferred { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }


    #endregion
}
