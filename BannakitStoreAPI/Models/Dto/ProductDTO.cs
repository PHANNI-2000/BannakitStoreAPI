using System;
using System.ComponentModel.DataAnnotations;

namespace BannakitStoreApi.Models.Dto
{
    public class ProductDTO
    {
        public int ProdId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string ProdNameTh { get; set; }
        [Required]
        public string ProdNameEn { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal? Costprice { get; set; }
        public bool ActiveStatus { get; set; }
        public string? ProdNo { get; set; }
        public string? DescTh { get; set; }
        public string? DescEn { get; set; }
        [Required]
        public int? Quatity { get; set; }
        [Required]
        public int? Available { get; set; }
        [Required]
        public int? Tax { get; set; }
        public string? Remark { get; set; }
    }
}
