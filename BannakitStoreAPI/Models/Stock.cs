using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace BannakitStoreApi.Models
{
    [Table("tbstock")]
    public class Stock
    {
        [Key]
        [Column("stock_id")]
        public int StockId { get; set; }
        [ForeignKey("Brand")]
        [Column("brand_id")]
        public int BrandId { get; set; }
        [ForeignKey("Product")]
        [Column("prod_id")]
        public int ProdId { get; set; }
        [Column("quatity")]
        public int Quatity { get; set; }
        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Column("updated_by")]
        public string UpdatedBy { get; set; }
        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; }

        public Brand Brand { get; set; }
        public Product Product { get; set; }
    }
}
