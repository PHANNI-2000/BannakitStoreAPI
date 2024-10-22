using System;

namespace BannakitStoreApi.Models.Dto
{
    public class OrderEntryDTO
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string ClientName { get; set; }
        public string ClientContact { get; set; }
        public string Address { get; set; }
        public int? BrandId { get; set; }
        public int? ProdId { get; set; }
        public int Quatity { get; set; }
        public int TotalAmount { get; set; }
        public int? AmountPaid { get; set; }
        public int DueAmount { get; set; }
        public int? PaymentTypeId { get; set; }
        public int PaymentStatusId { get; set; }
        public string Email { get; set; }
        public string OrderStatus { get; set; }
    }
}
