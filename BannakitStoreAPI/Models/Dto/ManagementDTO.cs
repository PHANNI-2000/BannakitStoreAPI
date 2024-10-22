using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace BannakitStoreApi.Models.Dto
{
    public class ManagementDTO
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public int ImageId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public decimal Salary { get; set; }
        public string Tel { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? IssueDate { get; set; }
        // Navigation Property
        public UserDTO User { get; set; }
        public DepartmentDTO Department { get; set; }
        public ImageDTO Image { get; set; }
    }
}
