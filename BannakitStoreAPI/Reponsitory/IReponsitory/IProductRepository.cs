using BannakitStoreApi.Models;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory.IReponsitory
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> UpdateAsync(Product entity);
        Task<string> GenerateNewProdNo();
    }
}
