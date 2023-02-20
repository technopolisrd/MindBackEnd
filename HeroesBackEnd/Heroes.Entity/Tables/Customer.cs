namespace Mind.Entity.Tables;

using Common.Core.Contracts.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Mind.Entity.SecurityAccount;

#nullable disable

[DataContract]
[Table("CODE_Customer")]
public class Customer : IAuditableEntity, IDeferrableEntity, IConcurrencyEntity
{
    #region Properties

    [DataMember]
    [Column("CustomerId")]
    [Display(Name = "Customer ID")]
    public int ID { get; set; }

    [DataMember]
    [StringLength(100, ErrorMessage = "Ha excedido la cantidad máxima de caracteres permitidos en este campo.")]
    [Display(Name = "Account Name")]
    public string AccountName { get; set; }

    [DataMember]
    [StringLength(100, ErrorMessage = "Ha excedido la cantidad máxima de caracteres permitidos en este campo.")]
    [Display(Name = "Customer Name")]
    public string CustomerName { get; set; }

    [DataMember]
    [Display(Name = "Responsible Operation Name")]
    public int AccountID { get; set; }

    #endregion

    #region Navigation
        
    public ICollection<Team> teams { get; set; }
    public ICollection<LogMovements> logMovements { get; set; }
    public Account account { get; set; }

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
