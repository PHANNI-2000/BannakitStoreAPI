using BannakitStoreApi.Models;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory.IReponsitory
{
    public interface IPaymentTypeRepository : IRepository<PaymentType>
    {
        Task<PaymentType> UpdateAsync(PaymentType entity);
    }
}
