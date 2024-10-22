using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace BannakitStoreApi.Models.Dto
{
    public class ImageDTO
    {
        public int ImageId { get; set; }
        public IFormFile File { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
        public string FileExtension { get; set; }
        public long FileSizeInBytes { get; set; }
        public string FilePath { get; set; }
    }
}
