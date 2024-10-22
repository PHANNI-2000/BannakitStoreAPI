namespace BannakitStoreApi.Models.Dto
{
    public class BrandEntryDTO
    {
        public int BrandId { get; set; }
        public string BrandNameTh { get; set; }
        public string BrandNameEn { get; set; }
        public bool? ActiveStatus { get; set; }
        public int? CategoryId { get; set; }
    }
}
