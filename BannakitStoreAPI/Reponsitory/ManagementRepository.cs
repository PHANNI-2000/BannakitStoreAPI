using BannakitStoreApi.Data;
using BannakitStoreApi.Models;
using BannakitStoreApi.Reponsitory.IReponsitory;
using System;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory
{
    public class ManagementRepository : Repository<Employee>, IManagementRepository
    {
        private readonly ApplicationDbContext _context;

        public ManagementRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Employee> UpdateAsync(Employee entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.Employees.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
