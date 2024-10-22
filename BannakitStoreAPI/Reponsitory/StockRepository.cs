using BannakitStoreApi.Data;
using BannakitStoreApi.Models;
using BannakitStoreApi.Reponsitory.IReponsitory;
using System;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory
{
    public class StockRepository : Repository<Stock>, IStockRepository
    {
        private readonly ApplicationDbContext _context;
        public StockRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Stock> UpdateAsync(Stock entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.Stocks.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
