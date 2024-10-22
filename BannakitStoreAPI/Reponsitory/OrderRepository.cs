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
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<string> GenerateNewOrder()
        {
            string newOrderNo = null;
            try
            {
                do
                {
                    newOrderNo = RandomOrderNo();
                } while (await _context.Orders.AnyAsync(o => o.OrderNo == newOrderNo)); // Check the database for duplicates.
            }
            catch (Exception ex)
            {
                throw;
            }

            return newOrderNo;
        }

        public async Task<Order> UpdateAsync(Order entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.Orders.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public static string RandomOrderNo()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder result = new StringBuilder(7);
            Random random = new Random();

            for (int i = 0; i < 7; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return $"BNK-{result}";
        }

    }
}
