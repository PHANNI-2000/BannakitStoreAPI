using BannakitStoreApi.Models;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory.IReponsitory
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category> UpdateAsync(Category entity);
    }
}
