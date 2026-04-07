using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Models;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Domain;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Factories
{
    public interface IAutoCouponModelFactory
    {
        Task<ConfigurationModel> PrepareConfigurationModelAsync();

        Task<IList<CouponGenerationLogModel>> PrepareLogListModelAsync(IList<CouponGenerationLog> logs);
    }
}