using BannakitStoreApi.Models;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory.IReponsitory
{
    public interface IImageRepository
    {
        Task<Image> Upload(Image image);
    }
}
