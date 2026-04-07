using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Services;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Consumers
{
    public class OrderPaidEventConsumer : IConsumer<OrderPaidEvent>
    {
        private readonly IAutoCouponService _autoCouponService;

        public OrderPaidEventConsumer(IAutoCouponService autoCouponService)
        {
            _autoCouponService = autoCouponService;
        }

        public async Task HandleEventAsync(OrderPaidEvent eventMessage)
        {
            var order = eventMessage.Order;

            if (order == null)
                return;

            await _autoCouponService.GenerateCouponAsync(order);
        }
    }
}