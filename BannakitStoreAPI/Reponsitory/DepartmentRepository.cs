using BannakitStoreApi.Data;
using BannakitStoreApi.Models;
using BannakitStoreApi.Reponsitory.IReponsitory;
using System.Threading.Tasks;
using System;

namespace BannakitStoreApi.Reponsitory
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Department> UpdateAsync(Department entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.Departments.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
