using BannakitStoreApi.Models;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory.IReponsitory
{
    public interface IBrandRepository : IRepository<Brand>
    {
        Task<Brand> UpdateAsync(Brand entity);
    }
}
