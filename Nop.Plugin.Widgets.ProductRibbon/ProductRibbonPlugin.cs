using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Widgets.ProductRibbon
{
    public class ProductRibbonPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        public ProductRibbonPlugin(
            IWebHelper webHelper,
            ILocalizationService localizationService,
            ISettingService settingService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _settingService = settingService;
        }

        public bool HideInWidgetList => false;

        public override async Task InstallAsync()
        {
            await _settingService.SaveSettingAsync(new ProductRibbonSettings
            {
                Enabled = false
            });

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Widgets.ProductRibbon.Menu.Main"] = "Product Ribbons",
                ["Plugins.Widgets.ProductRibbon.Menu.List"] = "Ribbon Management",
                ["Plugins.Widgets.ProductRibbon.Menu.Mapping"] = "Product Mappings",
                ["Plugins.Widgets.ProductRibbon.Menu.Settings"] = "Settings",

                ["Plugins.Widgets.ProductRibbon.Fields.Name"] = "Ribbon Text",
                ["Plugins.Widgets.ProductRibbon.Fields.BackgroundColor"] = "Background Color",
                ["Plugins.Widgets.ProductRibbon.Fields.TextColor"] = "Text Color",
                ["Plugins.Widgets.ProductRibbon.Fields.IsActive"] = "Active",
                ["Plugins.Widgets.ProductRibbon.Fields.Product"] = "Product",
                ["Plugins.Widgets.ProductRibbon.Fields.Ribbon"] = "Ribbon",
                ["Plugins.Widgets.ProductRibbon.Fields.PluginEnabled"] = "Plugin Enabled",

                ["Plugins.Widgets.ProductRibbon.Messages.RibbonCreated"] = "Ribbon created successfully.",
                ["Plugins.Widgets.ProductRibbon.Messages.RibbonUpdated"] = "Ribbon updated successfully.",
                ["Plugins.Widgets.ProductRibbon.Messages.RibbonDeleted"] = "Ribbon deleted successfully.",
                ["Plugins.Widgets.ProductRibbon.Messages.MappingCreated"] = "Product-ribbon mapping created successfully.",
                ["Plugins.Widgets.ProductRibbon.Messages.MappingDeleted"] = "Product-ribbon mapping deleted successfully.",
                ["Plugins.Widgets.ProductRibbon.Messages.SettingsSaved"] = "Settings saved successfully."
            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<ProductRibbonSettings>();
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.ProductRibbon");

            await base.UninstallAsync();
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ProductRibbon/List";
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            IList<string> widgetZones = new List<string>
            {
                PublicWidgetZones.ProductBoxAddinfoBefore
            };

            return Task.FromResult(widgetZones);
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsProductRibbon";
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var mainTitle = await _localizationService.GetResourceAsync("Plugins.Widgets.ProductRibbon.Menu.Main");
            var listTitle = await _localizationService.GetResourceAsync("Plugins.Widgets.ProductRibbon.Menu.List");
            var mappingTitle = await _localizationService.GetResourceAsync("Plugins.Widgets.ProductRibbon.Menu.Mapping");
            var settingsTitle = await _localizationService.GetResourceAsync("Plugins.Widgets.ProductRibbon.Menu.Settings");

            var menuItem = new SiteMapNode
            {
                SystemName = "ProductRibbon.Main",
                Title = string.IsNullOrWhiteSpace(mainTitle) ? "Product Ribbons" : mainTitle,
                Visible = true,
                IconClass = "far fa-dot-circle",
                ChildNodes = new List<SiteMapNode>
                {
                    new SiteMapNode
                    {
                        SystemName = "ProductRibbon.List",
                        Title = string.IsNullOrWhiteSpace(listTitle) ? "Ribbon Management" : listTitle,
                        ControllerName = "ProductRibbon",
                        ActionName = "List",
                        Visible = true,
                        IconClass = "far fa-circle",
                        RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
                    },
                    new SiteMapNode
                    {
                        SystemName = "ProductRibbon.Mapping",
                        Title = string.IsNullOrWhiteSpace(mappingTitle) ? "Product Mappings" : mappingTitle,
                        ControllerName = "ProductRibbon",
                        ActionName = "Mapping",
                        Visible = true,
                        IconClass = "far fa-circle",
                        RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
                    },
                    new SiteMapNode
                    {
                        SystemName = "ProductRibbon.Settings",
                        Title = string.IsNullOrWhiteSpace(settingsTitle) ? "Settings" : settingsTitle,
                        ControllerName = "ProductRibbon",
                        ActionName = "Settings",
                        Visible = true,
                        IconClass = "far fa-circle",
                        RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
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