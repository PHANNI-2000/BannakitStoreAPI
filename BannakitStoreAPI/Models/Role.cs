using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BannakitStoreApi.Models
{
    [Table("msrole")]
    public class Role
    {
        [Key]
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
        [Column("role_name_th")]
        public string RoleNameTh { get; set; }
        [Column("role_name_en")]
        public string RoleNameEn { get; set; }
        // Navigation Property
        public ICollection<User> Users { get; set; }  // relationship 1-to-many
    }
}
