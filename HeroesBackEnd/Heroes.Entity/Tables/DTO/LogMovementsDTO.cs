using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

#nullable disable

namespace Mind.Entity.Tables.DTO
{
    public class LogMovementsDTO
    {
        [DataMember]
        [Column("MovementId")]
        [Display(Name = "Movement ID")]
        public int ID { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public int CustomerID { get; set; }

        [DataMember]
        [StringLength(100, ErrorMessage = "Ha excedido la cantidad máxima de caracteres permitidos en este campo.")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [DataMember]
        [Display(Name = "User")]
        public int AccountID { get; set; }

        [DataMember]
        [StringLength(200, ErrorMessage = "Ha excedido la cantidad máxima de caracteres permitidos en este campo.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

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
    }
}
