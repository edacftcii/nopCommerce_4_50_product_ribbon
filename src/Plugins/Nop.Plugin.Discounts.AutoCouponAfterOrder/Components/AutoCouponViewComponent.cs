using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Models;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Components
{
    [ViewComponent(Name = "WidgetsAutoCouponAfterOrder")]
    public class AutoCouponViewComponent : NopViewComponent
    {
        private readonly IAutoCouponService _autoCouponService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        public AutoCouponViewComponent(
            IAutoCouponService autoCouponService,
            ILocalizationService localizationService,
            ISettingService settingService)
        {
            _autoCouponService = autoCouponService;
            _localizationService = localizationService;
            _settingService = settingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var settings = await _settingService.LoadSettingAsync<AutoCouponSettings>();
            if (settings == null || !settings.Enabled)
                return Content(string.Empty);

            if (additionalData == null)
                return Content(string.Empty);

            int orderId = 0;

            var orderIdProperty = additionalData.GetType().GetProperty("OrderId");
            if (orderIdProperty != null)
            {
                var orderIdValue = orderIdProperty.GetValue(additionalData);
                if (orderIdValue is int oid)
                    orderId = oid;
            }

            if (orderId <= 0)
            {
                var idProperty = additionalData.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    var idValue = idProperty.GetValue(additionalData);
                    if (idValue is int id)
                        orderId = id;
                }
            }

            if (orderId <= 0)
                return Content(string.Empty);

            var log = await _autoCouponService.GetLogByOrderIdAsync(orderId);
            if (log == null)
                return Content(string.Empty);

            var percentageText = await _localizationService.GetResourceAsync(
                "Plugins.Discounts.AutoCouponAfterOrder.DiscountTypes.Percentage");

            var fixedText = await _localizationService.GetResourceAsync(
                "Plugins.Discounts.AutoCouponAfterOrder.DiscountTypes.Fixed");

            var model = new PublicCouponNotificationModel
            {
                HasCoupon = true,
                CouponCode = log.CouponCode,
                DiscountValue = log.DiscountValue,
                DiscountTypeText = log.DiscountTypeId == 1 ? percentageText : fixedText,
                ExpirationDateUtc = log.ExpirationDateUtc
            };

            return View(
                "~/Plugins/Discounts.AutoCouponAfterOrder/Views/Shared/Components/AutoCoupon/Default.cshtml",
                model);
        }
    }
}