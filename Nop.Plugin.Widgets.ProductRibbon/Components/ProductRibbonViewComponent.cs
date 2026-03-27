using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.ProductRibbon.Services;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.ProductRibbon.Components
{
    [ViewComponent(Name = "WidgetsProductRibbon")]
    public class ProductRibbonViewComponent : NopViewComponent
    {
        private readonly IProductRibbonService _productRibbonService;
        private readonly ISettingService _settingService;

        public ProductRibbonViewComponent(
            IProductRibbonService productRibbonService,
            ISettingService settingService)
        {
            _productRibbonService = productRibbonService;
            _settingService = settingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var settings = await _settingService.LoadSettingAsync<ProductRibbonSettings>();
            if (!settings.Enabled)
                return Content(string.Empty);

            if (additionalData == null)
                return Content(string.Empty);

            var idProperty = additionalData.GetType().GetProperty("Id");
            if (idProperty == null)
                return Content(string.Empty);

            var idValue = idProperty.GetValue(additionalData);
            if (idValue is not int productId)
                return Content(string.Empty);

            var ribbon = await _productRibbonService.GetActiveRibbonByProductIdAsync(productId);

            if (ribbon == null)
                return Content(string.Empty);

            return View("~/Plugins/Widgets.ProductRibbon/Views/ProductRibbon/Default.cshtml", ribbon);
        }
    }
}