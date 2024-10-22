using BannakitStoreApi.Data;
using BannakitStoreApi.Models;
using BannakitStoreApi.Reponsitory.IReponsitory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product> UpdateAsync(Product entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.Products.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<string> GenerateNewProdNo()
        {
            var data = await _context.Products.Where(u => u.ProdNo.StartsWith("PD")).OrderByDescending(u => u.ProdNo).FirstOrDefaultAsync();
            string newProdNo = "PD001";

            if (data != null)
            {
                newProdNo = data.ProdNo.Substring(2);
                int lastestNumber = int.Parse(newProdNo);
                newProdNo = $"PD{(lastestNumber + 1).ToString("D3")}";

                return newProdNo;
            }

            return newProdNo;
        }
    }
}
