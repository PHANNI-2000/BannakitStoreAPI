using BannakitStoreApi.Data;
using BannakitStoreApi.Models;
using BannakitStoreApi.Reponsitory.IReponsitory;
using System;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Role> UpdateAsync(Role entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.Roles.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
