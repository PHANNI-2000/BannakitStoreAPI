using BannakitStoreApi.Models;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory.IReponsitory
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> UpdateAsync(Order entity);
        Task<string> GenerateNewOrder();
    }
}
