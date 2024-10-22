using System;
using Newtonsoft.Json;

namespace BannakitStoreApi.Models.Dto
{
    public class ManagementRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string CreatedBy { get; set; }
        //public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonConverter(typeof(JsonConverterExtension.CustomDateConverter))]
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public decimal Salary { get; set; }
        public string Tel { get; set; }
        [JsonConverter(typeof(JsonConverterExtension.CustomDateConverter))]
        public DateTime HireDate { get; set; }
        [JsonConverter(typeof(JsonConverterExtension.CustomDateConverter))]
        public DateTime? IssueDate { get; set; }
        public int DepartmentId { get; set; }
        public int? RoleId { get; set; }
    }
}
