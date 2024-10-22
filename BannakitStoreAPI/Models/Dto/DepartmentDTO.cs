using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models.Dto
{
    public class DepartmentDTO
    {
        public int DepartmentId { get; set; }
        public string DeptNameTh { get; set; }
        public string DeptNameEn { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
