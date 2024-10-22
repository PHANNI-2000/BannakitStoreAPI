using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models
{
    [Table("msdepartment")]
    public class Department
    {
        [Key]
        [Column("department_id")]
        public int DepartmentId { get; set; }
        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }
        [Column("updated_by")]
        public string UpdatedBy { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        [Column("dept_name_th")]
        public string DeptNameTh { get; set; }
        [Column("dept_name_en")]
        public string DeptNameEn { get; set; }
        [Column("active_status")]
        public bool ActiveStatus { get; set; }
    }
}
