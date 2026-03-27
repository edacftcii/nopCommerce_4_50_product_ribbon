using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Widgets.ProductRibbon.Models;
using Nop.Plugin.Widgets.ProductRibbon.Services;
using ProductRibbonEntity = Nop.Plugin.Widgets.ProductRibbon.Domain.ProductRibbon;
using ProductRibbonMappingEntity = Nop.Plugin.Widgets.ProductRibbon.Domain.ProductRibbonMapping;

namespace Nop.Plugin.Widgets.ProductRibbon.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class ProductRibbonController : BasePluginController
    {
        private readonly IProductRibbonService _productRibbonService;
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        public ProductRibbonController(
            IProductRibbonService productRibbonService,
            IProductService productService,
            INotificationService notificationService,
            ILocalizationService localizationService,
            ISettingService settingService)
        {
            _productRibbonService = productRibbonService;
            _productService = productService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _settingService = settingService;
        }

        public IActionResult List()
        {
            var model = new ProductRibbonSearchModel();
            return View("~/Plugins/Widgets.ProductRibbon/Views/ProductRibbon/List.cshtml", model);
        }

        public async Task<IActionResult> Settings()
        {
            var settings = await _settingService.LoadSettingAsync<ProductRibbonSettings>();

            var model = new ProductRibbonSearchModel
            {
                PluginEnabled = settings.Enabled
            };

            return View("~/Plugins/Widgets.ProductRibbon/Views/ProductRibbon/Settings.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettings(ProductRibbonSearchModel model)
        {
            var settings = await _settingService.LoadSettingAsync<ProductRibbonSettings>();
            settings.Enabled = model.PluginEnabled;

            await _settingService.SaveSettingAsync(settings);

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Plugins.Widgets.ProductRibbon.Messages.SettingsSaved"));

            return RedirectToAction(nameof(Settings));
        }

        [HttpPost]
        public async Task<IActionResult> ListData()
        {
            var ribbons = await _productRibbonService.GetAllRibbonsAsync();

            var data = ribbons.Select(x => new ProductRibbonModel
            {
                Id = x.Id,
                Name = x.Name,
                BackgroundColor = x.BackgroundColor,
                TextColor = x.TextColor,
                IsActive = x.IsActive
            }).ToList();

            var draw = Request.Form["draw"].FirstOrDefault();

            return Json(new
            {
                Draw = draw,
                RecordsTotal = data.Count,
                RecordsFiltered = data.Count,
                Data = data
            });
        }

        public IActionResult Create()
        {
            var model = new ProductRibbonModel
            {
                BackgroundColor = "#e74c3c",
                TextColor = "#ffffff",
                IsActive = false
            };

            return View("~/Plugins/Widgets.ProductRibbon/Views/ProductRibbon/Create.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductRibbonModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Plugins/Widgets.ProductRibbon/Views/ProductRibbon/Create.cshtml", model);

            var entity = new ProductRibbonEntity
            {
                Name = model.Name,
                BackgroundColor = model.BackgroundColor,
                TextColor = model.TextColor,
                IsActive = model.IsActive
            };

            await _productRibbonService.InsertRibbonAsync(entity);

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Plugins.Widgets.ProductRibbon.Messages.RibbonCreated"));

            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ribbon = await _productRibbonService.GetRibbonByIdAsync(id);
            if (ribbon == null)
                return RedirectToAction(nameof(List));

            var model = new ProductRibbonModel
            {
                Id = ribbon.Id,
                Name = ribbon.Name,
                BackgroundColor = ribbon.BackgroundColor,
                TextColor = ribbon.TextColor,
                IsActive = ribbon.IsActive
            };

            return View("~/Plugins/Widgets.ProductRibbon/Views/ProductRibbon/Edit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductRibbonModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Plugins/Widgets.ProductRibbon/Views/ProductRibbon/Edit.cshtml", model);

            var ribbon = await _productRibbonService.GetRibbonByIdAsync(model.Id);
            if (ribbon == null)
                return RedirectToAction(nameof(List));

            ribbon.Name = model.Name;
            ribbon.BackgroundColor = model.BackgroundColor;
            ribbon.TextColor = model.TextColor;
            ribbon.IsActive = model.IsActive;

            await _productRibbonService.UpdateRibbonAsync(ribbon);

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Plugins.Widgets.ProductRibbon.Messages.RibbonUpdated"));

            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var ribbon = await _productRibbonService.GetRibbonByIdAsync(id);
            if (ribbon != null)
            {
                var mappings = await _productRibbonService.GetMappingsByRibbonIdAsync(id);

                foreach (var mapping in mappings)
                    await _productRibbonService.DeleteMappingAsync(mapping);

                await _productRibbonService.DeleteRibbonAsync(ribbon);

                _notificationService.SuccessNotification(
                    await _localizationService.GetResourceAsync("Plugins.Widgets.ProductRibbon.Messages.RibbonDeleted"));
            }

            return Json(new { Result = true });
        }

        public async Task<IActionResult> Mapping()
        {
            var model = new ProductRibbonMappingModel();
            await PrepareMappingModelAsync(model);

            return View("~/Plugins/Widgets.ProductRibbon/Views/ProductRibbon/Mapping.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Mapping(ProductRibbonMappingModel model)
        {
            if (model.ProductId > 0 && model.ProductRibbonId > 0)
            {
                var existingMappings = await _productRibbonService.GetMappingsByProductIdAsync(model.ProductId);

                foreach (var existing in existingMappings)
                    await _productRibbonService.DeleteMappingAsync(existing);

                var mapping = new ProductRibbonMappingEntity
                {
                    ProductId = model.ProductId,
                    ProductRibbonId = model.ProductRibbonId
                };

                await _productRibbonService.InsertMappingAsync(mapping);

                _notificationService.SuccessNotification(
                    await _localizationService.GetResourceAsync("Plugins.Widgets.ProductRibbon.Messages.MappingCreated"));
            }

            await PrepareMappingModelAsync(model);

            return View("~/Plugins/Widgets.ProductRibbon/Views/ProductRibbon/Mapping.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> MappingListData()
        {
            var ribbons = await _productRibbonService.GetAllRibbonsAsync();
            var result = new List<ProductRibbonMappingListItemModel>();

            foreach (var ribbon in ribbons)
            {
                var mappings = await _productRibbonService.GetMappingsByRibbonIdAsync(ribbon.Id);

                foreach (var mapping in mappings)
                {
                    var product = await _productService.GetProductByIdAsync(mapping.ProductId);

                    result.Add(new ProductRibbonMappingListItemModel
                    {
                        Id = mapping.Id,
                        ProductId = mapping.ProductId,
                        ProductName = product?.Name ?? $"Product #{mapping.ProductId}",
                        ProductRibbonId = ribbon.Id,
                        RibbonName = ribbon.Name
                    });
                }
            }

            var ordered = result.OrderBy(x => x.ProductName).ToList();
            var draw = Request.Form["draw"].FirstOrDefault();

            return Json(new
            {
                Draw = draw,
                RecordsTotal = ordered.Count,
                RecordsFiltered = ordered.Count,
                Data = ordered
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMapping(int id)
        {
            var allRibbons = await _productRibbonService.GetAllRibbonsAsync();

            foreach (var ribbon in allRibbons)
            {
                var mappings = await _productRibbonService.GetMappingsByRibbonIdAsync(ribbon.Id);
                var mapping = mappings.FirstOrDefault(x => x.Id == id);

                if (mapping != null)
                {
                    await _productRibbonService.DeleteMappingAsync(mapping);

                    _notificationService.SuccessNotification(
                        await _localizationService.GetResourceAsync("Plugins.Widgets.ProductRibbon.Messages.MappingDeleted"));

                    break;
                }
            }

            return Json(new { Result = true });
        }

        private async Task PrepareMappingModelAsync(ProductRibbonMappingModel model)
        {
            var products = await _productService.SearchProductsAsync(pageSize: 200);
            var ribbons = await _productRibbonService.GetAllRibbonsAsync();

            model.AvailableProducts = products
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();

            model.AvailableRibbons = ribbons
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();
        }
    }
}