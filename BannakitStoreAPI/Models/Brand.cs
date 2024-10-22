using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models
{
    [Table("tbbrand")]
    public class Brand
    {
        [Key]
        [Column("brand_id")]
        public int BrandId { get; set; }
        [ForeignKey("Category")]
        [Column("category_id")]
        public int? CategoryId { get; set; }
        [Column("brand_name_th")]
        public string BrandNameTh { get; set; }
        [Column("brand_name_en")]
        public string BrandNameEn { get; set; }
        [Column("active_status")]
        public bool? ActiveStatus { get; set; }
        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }
        [Column("updated_by")]
        public string UpdatedBy { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        public Category Category { get; set; }
    }
}
