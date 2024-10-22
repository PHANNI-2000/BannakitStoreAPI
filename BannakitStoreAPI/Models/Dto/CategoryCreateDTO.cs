using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models.Dto
{
    public class CategoryCreateDTO
    {
        //public string CreatedBy { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public string UpdatedBy { get; set; }
        //public DateTime UpdatedDate { get; set; }
        public string CategoryNameTh { get; set; }
        public string CategoryNameEn { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
