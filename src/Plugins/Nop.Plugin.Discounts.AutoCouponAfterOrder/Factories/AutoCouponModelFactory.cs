using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Domain;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Factories
{
    public class AutoCouponModelFactory : IAutoCouponModelFactory
    {
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public AutoCouponModelFactory(
            ISettingService settingService,
            ILocalizationService localizationService)
        {
            _settingService = settingService;
            _localizationService = localizationService;
        }

public async Task<ConfigurationModel> PrepareConfigurationModelAsync()
{
    var settings = await _settingService.LoadSettingAsync<AutoCouponSettings>();

    var selectedType = settings.DiscountTypeId;
    if (selectedType != 1 && selectedType != 2)
        selectedType = 1;

    var model = new ConfigurationModel
    {
        Enabled = settings.Enabled,
        DiscountTypeId = selectedType,
        DiscountValue = settings.DiscountValue,
        ValidDays = settings.ValidDays,
        AvailableDiscountTypes = new List<SelectListItem>
        {
            new SelectListItem
            {
                Value = "1",
                Text = "Percentage",
                Selected = selectedType == 1
            },
            new SelectListItem
            {
                Value = "2",
                Text = "Fixed amount",
                Selected = selectedType == 2
            }
        }
    };

    return model;
}

        public async Task<IList<CouponGenerationLogModel>> PrepareLogListModelAsync(IList<CouponGenerationLog> logs)
        {
            var percentageText = await _localizationService.GetResourceAsync("Plugins.Discounts.AutoCouponAfterOrder.DiscountTypes.Percentage");
            var fixedText = await _localizationService.GetResourceAsync("Plugins.Discounts.AutoCouponAfterOrder.DiscountTypes.Fixed");

            return logs.Select(x => new CouponGenerationLogModel
            {
                Id = x.Id,
                OrderId = x.OrderId,
                CustomerId = x.CustomerId,
                DiscountId = x.DiscountId,
                CouponCode = x.CouponCode,
                DiscountValue = x.DiscountValue,
                DiscountTypeName = x.DiscountTypeId == 1 ? percentageText : fixedText,
                CreatedOnUtc = x.CreatedOnUtc,
                ExpirationDateUtc = x.ExpirationDateUtc
            }).ToList();
        }
    }
}