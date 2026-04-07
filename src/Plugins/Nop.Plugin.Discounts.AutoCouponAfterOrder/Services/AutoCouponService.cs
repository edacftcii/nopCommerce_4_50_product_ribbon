using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Domain;
using Nop.Services.Configuration;
using Nop.Core.Domain.Payments;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Services
{
    public class AutoCouponService : IAutoCouponService
    {
        private readonly IRepository<CouponGenerationLog> _couponGenerationLogRepository;
        private readonly IRepository<Discount> _discountRepository;
        private readonly ISettingService _settingService;

        public AutoCouponService(
            IRepository<CouponGenerationLog> couponGenerationLogRepository,
            IRepository<Discount> discountRepository,
            ISettingService settingService)
        {
            _couponGenerationLogRepository = couponGenerationLogRepository;
            _discountRepository = discountRepository;
            _settingService = settingService;
        }

        public async Task<IList<CouponGenerationLog>> GetAllLogsAsync()
        {
            return await _couponGenerationLogRepository.GetAllAsync(query =>
                query.OrderByDescending(x => x.CreatedOnUtc));
        }

        public async Task<CouponGenerationLog> GetLogByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
                return null;

            var logs = await _couponGenerationLogRepository.GetAllAsync(query =>
                query.Where(x => x.OrderId == orderId)
                     .OrderByDescending(x => x.CreatedOnUtc));

            return logs.FirstOrDefault();
        }

        public async Task InsertLogAsync(CouponGenerationLog log)
        {
            await _couponGenerationLogRepository.InsertAsync(log);
        }

public async Task GenerateCouponAsync(Order order)
{
    if (order == null)
        return;

    if (order.PaymentStatus != PaymentStatus.Paid)
        return;

    var settings = await _settingService.LoadSettingAsync<AutoCouponSettings>();

    if (settings == null || !settings.Enabled)
        return;

    if (settings.DiscountValue <= 0 || settings.ValidDays <= 0)
        return;

    var existingLogs = await _couponGenerationLogRepository.GetAllAsync(query =>
        query.Where(x => x.OrderId == order.Id));

    if (existingLogs.Any())
        return;

    var nowUtc = DateTime.UtcNow;
    var expirationDateUtc = nowUtc.AddDays(settings.ValidDays);
    var couponCode = $"GIFT-{order.Id}-{Guid.NewGuid().ToString("N")[..4].ToUpper()}";

    var discount = new Discount
    {
        Name = $"Auto coupon for order #{order.Id}",
        DiscountTypeId = (int)DiscountType.AssignedToOrderSubTotal,
        UsePercentage = settings.DiscountTypeId == 1,
        DiscountPercentage = settings.DiscountTypeId == 1 ? settings.DiscountValue : decimal.Zero,
        DiscountAmount = settings.DiscountTypeId == 2 ? settings.DiscountValue : decimal.Zero,
        StartDateUtc = nowUtc,
        EndDateUtc = expirationDateUtc,
        RequiresCouponCode = true,
        CouponCode = couponCode,
        IsCumulative = false,
        LimitationTimes = 1,
        DiscountLimitationId = (int)DiscountLimitationType.NTimesOnly,
        MaximumDiscountedQuantity = null,
        AppliedToSubCategories = false
    };

    await _discountRepository.InsertAsync(discount);

    var log = new CouponGenerationLog
    {
        OrderId = order.Id,
        CustomerId = order.CustomerId,
        DiscountId = discount.Id,
        CouponCode = couponCode,
        DiscountValue = settings.DiscountValue,
        DiscountTypeId = settings.DiscountTypeId,
        CreatedOnUtc = nowUtc,
        ExpirationDateUtc = expirationDateUtc
    };

    await InsertLogAsync(log);
}
    }
}