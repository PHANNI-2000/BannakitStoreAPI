using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models
{
    [Table("tbproduct")]
    public class Product
    {
        [Key]
        [Column("prod_id")]
        public int ProdId { get; set; }
        [ForeignKey("Brand")]
        [Column("brand_id")]
        public int BrandId { get; set; }
        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Column("updated_by")]
        public string UpdatedBy { get; set; }
        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; }
        [ForeignKey("Category")]
        [Column("category_id")]
        public int CategoryId { get; set; }
        [Column("prod_name_th")]
        public string ProdNameTh { get; set; }
        [Column("prod_name_en")]
        public string ProdNameEn { get; set; }
        [Column("price")]
        public decimal Price { get; set; }
        [Column("costprice")]
        public decimal? Costprice { get; set; }
        [Column("active_status")]
        public bool ActiveStatus { get; set; }
        [Column("prod_no")]
        public string? ProdNo { get; set; }
        [Column("desc_th")]
        public string? DescTh { get; set; }
        [Column("desc_en")]
        public string? DescEn { get; set; }
        [Column("quatity")]
        public int? Quatity { get; set; }
        [Column("available")]
        public int? Available { get; set; }
        [Column("tax")]
        public int? Tax { get; set; }
        [Column("remark")]
        public string? Remark { get; set; }
        // Navigation Property for Category
        public Category Category { get; set; }
        public Brand Brand { get; set; }
    }
}
