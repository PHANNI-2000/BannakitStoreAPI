using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models
{
    [Table("tbimage")]
    public class Image
    {
        [Key]
        [Column("image_id")]
        public int ImageId { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        [Column("file_name")]
        public string FileName { get; set; }
        [Column("file_description")]
        public string FileDescription { get; set; }
        [Column("file_extension")]
        public string FileExtension { get; set; }
        [Column("file_sizeinbytes")]
        public long FileSizeInBytes { get; set; }
        [Column("file_path")]
        public string FilePath { get; set; }
    }
}
