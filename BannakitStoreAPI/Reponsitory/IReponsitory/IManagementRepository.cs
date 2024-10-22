using BannakitStoreApi.Models;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory.IReponsitory
{
    public interface IManagementRepository : IRepository<Employee>
    {
        Task<Employee> UpdateAsync(Employee entity);
    }
}
