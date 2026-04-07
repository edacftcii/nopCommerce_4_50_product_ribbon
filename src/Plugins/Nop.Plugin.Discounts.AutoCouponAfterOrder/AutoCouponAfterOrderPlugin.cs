using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Infrastructure;
using Nop.Data;
using Nop.Core.Domain.Cms;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder
{
    public class AutoCouponAfterOrderPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INopDataProvider _dataProvider;

        public AutoCouponAfterOrderPlugin(
            IWebHelper webHelper,
            ISettingService settingService,
            ILocalizationService localizationService,
            INopDataProvider dataProvider)
        {
            _webHelper = webHelper;
            _settingService = settingService;
            _localizationService = localizationService;
            _dataProvider = dataProvider;
        }

        public bool HideInWidgetList => false;

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AutoCoupon/Configure";
        }

public Task<IList<string>> GetWidgetZonesAsync()
{
    IList<string> widgetZones = new List<string>
    {
        PublicWidgetZones.CheckoutCompletedTop,
        PublicWidgetZones.OrderDetailsPageTop
    };

    return Task.FromResult(widgetZones);
}

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsAutoCouponAfterOrder";
        }

        public override async Task InstallAsync()
        {
            await _settingService.SaveSettingAsync(new AutoCouponSettings
            {
                Enabled = false,
                DiscountTypeId = 1,
                DiscountValue = 10,
                ValidDays = 30
            });

             var widgetSettings = await _settingService.LoadSettingAsync<WidgetSettings>();
    if (!widgetSettings.ActiveWidgetSystemNames.Contains("Discounts.AutoCouponAfterOrder"))
    {
        widgetSettings.ActiveWidgetSystemNames.Add("Discounts.AutoCouponAfterOrder");
        await _settingService.SaveSettingAsync(widgetSettings);
    }

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Discounts.AutoCouponAfterOrder.Menu.Main"] = "Auto Coupon After Order",
                ["Plugins.Discounts.AutoCouponAfterOrder.Menu.Configuration"] = "Configuration",
                ["Plugins.Discounts.AutoCouponAfterOrder.Menu.Logs"] = "Coupon Logs",

                ["Plugins.Discounts.AutoCouponAfterOrder.Fields.Enabled"] = "Enabled",
                ["Plugins.Discounts.AutoCouponAfterOrder.Fields.DiscountType"] = "Discount type",
                ["Plugins.Discounts.AutoCouponAfterOrder.Fields.DiscountValue"] = "Discount value",
                ["Plugins.Discounts.AutoCouponAfterOrder.Fields.ValidDays"] = "Valid days",

                ["Plugins.Discounts.AutoCouponAfterOrder.Fields.OrderId"] = "Order Id",
                ["Plugins.Discounts.AutoCouponAfterOrder.Fields.CustomerId"] = "Customer Id",
                ["Plugins.Discounts.AutoCouponAfterOrder.Fields.DiscountId"] = "Discount Id",
                ["Plugins.Discounts.AutoCouponAfterOrder.Fields.CouponCode"] = "Coupon code",
                ["Plugins.Discounts.AutoCouponAfterOrder.Fields.DiscountTypeName"] = "Discount type",
                ["Plugins.Discounts.AutoCouponAfterOrder.Fields.CreatedOnUtc"] = "Created on",
                ["Plugins.Discounts.AutoCouponAfterOrder.Fields.ExpirationDateUtc"] = "Expiration date",

                ["Plugins.Discounts.AutoCouponAfterOrder.DiscountTypes.Percentage"] = "Percentage",
                ["Plugins.Discounts.AutoCouponAfterOrder.DiscountTypes.Fixed"] = "Fixed amount",

                ["Plugins.Discounts.AutoCouponAfterOrder.Messages.SettingsSaved"] = "Settings saved successfully.",
                ["Plugins.Discounts.AutoCouponAfterOrder.Public.Title"] = "Your coupon is ready",
                ["Plugins.Discounts.AutoCouponAfterOrder.Public.Message"] = "Use this coupon on your next order:",
                ["Plugins.Discounts.AutoCouponAfterOrder.Public.Expiration"] = "Expiration date"
            });

            await base.InstallAsync();
        }

public override async Task UninstallAsync()
{
    await _settingService.DeleteSettingAsync<AutoCouponSettings>();

    var widgetSettings = await _settingService.LoadSettingAsync<WidgetSettings>();
    if (widgetSettings.ActiveWidgetSystemNames.Contains("Discounts.AutoCouponAfterOrder"))
    {
        widgetSettings.ActiveWidgetSystemNames.Remove("Discounts.AutoCouponAfterOrder");
        await _settingService.SaveSettingAsync(widgetSettings);
    }

    await _localizationService.DeleteLocaleResourcesAsync("Plugins.Discounts.AutoCouponAfterOrder");

    await _dataProvider.ExecuteNonQueryAsync(@"
        IF OBJECT_ID(N'[dbo].[CouponGenerationLog]', N'U') IS NOT NULL
            DROP TABLE [dbo].[CouponGenerationLog];
    ");

    await base.UninstallAsync();
}

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var mainTitle = await _localizationService.GetResourceAsync("Plugins.Discounts.AutoCouponAfterOrder.Menu.Main");
            var configurationTitle = await _localizationService.GetResourceAsync("Plugins.Discounts.AutoCouponAfterOrder.Menu.Configuration");
            var logsTitle = await _localizationService.GetResourceAsync("Plugins.Discounts.AutoCouponAfterOrder.Menu.Logs");

            var menuItem = new SiteMapNode
            {
                SystemName = "AutoCouponAfterOrder.Main",
                Title = string.IsNullOrWhiteSpace(mainTitle) ? "Auto Coupon After Order" : mainTitle,
                Visible = true,
                IconClass = "far fa-dot-circle",
                ChildNodes = new List<SiteMapNode>
                {
                    new SiteMapNode
                    {
                        SystemName = "AutoCouponAfterOrder.Configuration",
                        Title = string.IsNullOrWhiteSpace(configurationTitle) ? "Configuration" : configurationTitle,
                        ControllerName = "AutoCoupon",
                        ActionName = "Configure",
                        Visible = true,
                        IconClass = "far fa-circle",
                        RouteValues = new RouteValueDictionary
                        {
                            { "area", AreaNames.Admin }
                        }
                    },
                    new SiteMapNode
                    {
                        SystemName = "AutoCouponAfterOrder.Logs",
                        Title = string.IsNullOrWhiteSpace(logsTitle) ? "Coupon Logs" : logsTitle,
                        ControllerName = "AutoCoupon",
                        ActionName = "Logs",
                        Visible = true,
                        IconClass = "far fa-circle",
                        RouteValues = new RouteValueDictionary
                        {
                            { "area", AreaNames.Admin }
                        }
                    }
                }
            };

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");

            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);
        }
    }
}