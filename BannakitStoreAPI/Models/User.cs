using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models
{
    [Table("msuser")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }
        [ForeignKey("Role")]
        [Column("role_id")]
        public int RoleId { get; set; }
        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }
        [Column("updated_by")]
        public string UpdatedBy { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("password")]
        public string Password { get; set; }

        // Navigation Property for Role
        public Role Role { get; set; }
    }
}
