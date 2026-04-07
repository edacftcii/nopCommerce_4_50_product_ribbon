using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Domain;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Services
{
    public interface IAutoCouponService
    {
        Task<IList<CouponGenerationLog>> GetAllLogsAsync();

        Task<CouponGenerationLog> GetLogByOrderIdAsync(int orderId);

        Task InsertLogAsync(CouponGenerationLog log);

        Task GenerateCouponAsync(Order order);
    }
}