using System;

namespace BannakitStoreApi.Models.Dto
{
    public class PaymentTypeEntryDTO
    {
        public int PaymentTypeId { get; set; }
        public string PaymentNameTh { get; set; }
        public string PaymentNameEn { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
