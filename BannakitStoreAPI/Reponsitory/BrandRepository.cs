using BannakitStoreApi.Data;
using BannakitStoreApi.Models;
using BannakitStoreApi.Reponsitory.IReponsitory;
using System;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly ApplicationDbContext _context;

        public BrandRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Brand> UpdateAsync(Brand entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.brands.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}