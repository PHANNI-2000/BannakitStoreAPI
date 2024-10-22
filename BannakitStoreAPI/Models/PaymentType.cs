using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models
{
    [Table("mspaymenttype")]
    public class PaymentType
    {
        [Key]
        [Column("payment_type_id")]
        public int PaymentTypeId { get; set; }
        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }
        [Column("updated_by")]
        public string UpdatedBy { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        [Column("payment_name_th")]
        public string PaymentNameTh { get; set; }
        [Column("payment_name_en")]
        public string PaymentNameEn { get; set; }
        [Column("active_status")]
        public bool ActiveStatus { get; set; }
    }
}
