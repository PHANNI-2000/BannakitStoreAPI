using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models.Dto
{
    public class CategoryUpdateDTO
    {
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string UpdatedBy { get; set; }
        [Required]
        public DateTime UpdatedDate { get; set; }
        [Required]
        public string CategoryNameTh { get; set; }
        [Required]
        public string CategoryNameEn { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
