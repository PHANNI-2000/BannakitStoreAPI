using BannakitStoreApi.Data;
using BannakitStoreApi.Models;
using BannakitStoreApi.Reponsitory.IReponsitory;
using System;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory
{
    public class PaymentTypeRepository : Repository<PaymentType>, IPaymentTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaymentType> UpdateAsync(PaymentType entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.paymentTypes.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
