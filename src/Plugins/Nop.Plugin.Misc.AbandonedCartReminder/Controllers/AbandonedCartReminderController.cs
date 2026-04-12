using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.AbandonedCartReminder.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Misc.AbandonedCartReminder.Factories;

namespace Nop.Plugin.Misc.AbandonedCartReminder.Controllers;

[AuthorizeAdmin]
[Area("Admin")]
[AutoValidateAntiforgeryToken]
public class AbandonedCartReminderController : BasePluginController
{
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IAbandonedCartReminderModelFactory _abandonedCartReminderModelFactory;

    public AbandonedCartReminderController(
        ILocalizationService localizationService,
        ISettingService settingService,
        IAbandonedCartReminderModelFactory abandonedCartReminderModelFactory)
    {
        _localizationService = localizationService;
        _settingService = settingService;
        _abandonedCartReminderModelFactory = abandonedCartReminderModelFactory;
    }

public async Task<IActionResult> Configure()
{
    var model = await _abandonedCartReminderModelFactory.PrepareConfigurationModelAsync();
    return View("~/Plugins/Misc.AbandonedCartReminder/Views/Configure.cshtml", model);
}

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        var settings = await _settingService.LoadSettingAsync<AbandonedCartReminderSettings>();

        settings.SendAfterHours = model.SendAfterHours;
        settings.Enabled = model.Enabled;

        await _settingService.SaveSettingAsync(settings);

        TempData["success"] = await _localizationService.GetResourceAsync("Admin.Plugins.Saved");

        return await Configure();
    }
}