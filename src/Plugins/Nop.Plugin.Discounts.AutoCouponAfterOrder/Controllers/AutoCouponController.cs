using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Factories;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Models;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class AutoCouponController : BasePluginController
    {
        private readonly IAutoCouponService _autoCouponService;
        private readonly IAutoCouponModelFactory _autoCouponModelFactory;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        public AutoCouponController(
            IAutoCouponService autoCouponService,
            IAutoCouponModelFactory autoCouponModelFactory,
            ISettingService settingService,
            INotificationService notificationService,
            ILocalizationService localizationService)
        {
            _autoCouponService = autoCouponService;
            _autoCouponModelFactory = autoCouponModelFactory;
            _settingService = settingService;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        public async Task<IActionResult> Configure()
        {
            var model = await _autoCouponModelFactory.PrepareConfigurationModelAsync();
            return View("~/Plugins/Discounts.AutoCouponAfterOrder/Views/AutoCoupon/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            var settings = await _settingService.LoadSettingAsync<AutoCouponSettings>();

            settings.Enabled = model.Enabled;
            settings.DiscountTypeId = model.DiscountTypeId;
            settings.DiscountValue = model.DiscountValue;
            settings.ValidDays = model.ValidDays;

            await _settingService.SaveSettingAsync(settings);

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Plugins.Discounts.AutoCouponAfterOrder.Messages.SettingsSaved"));

            return RedirectToAction(nameof(Configure));
        }

        public IActionResult Logs()
        {
            var model = new CouponGenerationLogSearchModel();
            return View("~/Plugins/Discounts.AutoCouponAfterOrder/Views/AutoCoupon/LogList.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> LogListData()
        {
            var logs = await _autoCouponService.GetAllLogsAsync();
            var data = await _autoCouponModelFactory.PrepareLogListModelAsync(logs);
            var draw = Request.Form["draw"];

            return Json(new
            {
                Draw = draw,
                RecordsTotal = data.Count,
                RecordsFiltered = data.Count,
                Data = data
            });
        }
    }
}