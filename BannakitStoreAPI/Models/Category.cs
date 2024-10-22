using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models
{
    [Table("mscategory")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }
        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Column("updated_by")]
        public string UpdatedBy { get; set; }
        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; }
        [Column("category_name_th")]
        public string CategoryNameTh { get; set; }
        [Column("category_name_en")]
        public string CategoryNameEn { get; set; }
        [Column("active_status")]
        public bool ActiveStatus { get; set; }

        //public ICollection<Brand> Brands { get; set; }
    }
}
