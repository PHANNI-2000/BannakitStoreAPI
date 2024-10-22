using BannakitStoreApi.Models;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory.IReponsitory
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role> UpdateAsync(Role entity);
    }
}
