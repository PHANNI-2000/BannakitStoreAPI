using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models
{
    [Table("tborder")]
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }
        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }
        [Column("updated_by")]
        public string UpdatedBy { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        [Column("order_date")]
        public DateTime? OrderDate { get; set; }
        [Column("client_name")]
        public string ClientName { get; set; }
        [Column("client_contact")]
        public string ClientContact { get; set; }
        [Column("address")]
        public string Address { get; set; }
        [ForeignKey("Brand")]
        [Column("brand_id")]
        public int? BrandId { get; set; }
        [ForeignKey("Product")]
        [Column("prod_id")]
        public int? ProdId { get; set; }
        [Column("quatity")]
        public int Quatity { get; set; }
        [Column("total_amount")]
        public int TotalAmount { get; set; }
        [Column("amount_paid")]
        public int? AmountPaid { get; set; }
        [Column("due_amount")]
        public int DueAmount { get; set; }
        [ForeignKey("PaymentType")]
        [Column("payment_type_id")]
        public int? PaymentTypeId { get; set; }
        [Column("payment_status_th")]
        public string PaymentStatusTh { get; set; }
        [Column("payment_status_en")]
        public string PaymentStatusEn { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("order_no")]
        public string OrderNo { get; set; }
        [Column("order_status")]
        public string OrderStatus { get; set; }

        // Navigation
        public Brand Brand { get; set; }
        public Product Product { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}
