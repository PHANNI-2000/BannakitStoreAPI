using System;
using System.ComponentModel.DataAnnotations;

namespace BannakitStoreApi.Models.Dto
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        [Required]
        public string CategoryNameTh { get; set; }
        [Required]
        public string CategoryNameEn { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
