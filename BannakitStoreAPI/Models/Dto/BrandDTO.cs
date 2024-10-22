using System.ComponentModel.DataAnnotations;

namespace BannakitStoreApi.Models.Dto
{
    public class BrandDTO
    {
        public int BrandId { get; set; }
        [Required]
        public string BrandNameTh { get; set; }
        [Required]
        public string BrandNameEn { get; set; }
        public bool? ActiveStatus { get; set; }
        [Required]
        public CategoryDTO Category { get; set; }
    }
}
