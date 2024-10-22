namespace BannakitStoreApi.Models.Dto
{
    public class DepartmentEntryDTO
    {
        public int DepartmentId { get; set; }
        public string DeptNameTh { get; set; }
        public string DeptNameEn { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
