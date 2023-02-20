using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

#nullable disable

namespace Mind.Entity.Tables.DTO
{
    public class CustomerDTO
    {
        [DataMember]
        [Column("CustomerId")]
        [Display(Name = "Customer ID")]
        public string ID { get; set; }

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
        public string AccountID { get; set; }

        [DataMember]
        [StringLength(200, ErrorMessage = "Ha excedido la cantidad máxima de caracteres permitidos en este campo.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}
