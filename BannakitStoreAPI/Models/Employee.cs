using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models
{
    [Table("msemployee")]
    public class Employee
    {
        [Key]
        [Column("employee_id")]
        public int EmployeeId { get; set; }
        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        [ForeignKey("Department")]
        [Column("department_id")]
        public int DepartmentId { get; set; }
        [ForeignKey("Image")]
        [Column("image_id")]
        public int ImageId { get; set; }
        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }
        [Column("updated_by")]
        public string UpdatedBy { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }
        [Column("gender")]
        public string Gender { get; set; }
        [Column("salary")]
        public decimal Salary { get; set; }
        [Column("tel")]
        public string Tel { get; set; }
        [Column("hire_date")]
        public DateTime HireDate { get; set; }
        [Column("issue_date")]
        public DateTime? IssueDate { get; set; }
        // Navigation Property for User
        public User User { get; set; }
        public Department Department { get; set; }
        public Image Image { get; set; }
    }
}
