using System.ComponentModel.DataAnnotations;

namespace BannakitStoreApi.Models.Dto
{
    public class PaymentTypeDTO
    {
        [Required]
        public string PaymentNameTh { get; set; }
        [Required]
        public string PaymentNameEn { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
