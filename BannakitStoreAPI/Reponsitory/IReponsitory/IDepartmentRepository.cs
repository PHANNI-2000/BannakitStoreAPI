using BannakitStoreApi.Models;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory.IReponsitory
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<Department> UpdateAsync(Department entity);
    }
}
