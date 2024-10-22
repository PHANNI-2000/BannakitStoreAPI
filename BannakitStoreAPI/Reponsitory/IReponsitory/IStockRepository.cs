using BannakitStoreApi.Models;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory.IReponsitory
{
    public interface IStockRepository : IRepository<Stock>
    {
        Task<Stock> UpdateAsync(Stock entity);
    }
}
