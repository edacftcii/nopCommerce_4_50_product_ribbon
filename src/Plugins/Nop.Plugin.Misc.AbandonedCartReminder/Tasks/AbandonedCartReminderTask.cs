using Nop.Plugin.Misc.AbandonedCartReminder.Services;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.AbandonedCartReminder.Tasks;

public class AbandonedCartReminderTask : IScheduleTask
{
    private readonly ISettingService _settingService;
    private readonly ILogger _logger;
    private readonly IAbandonedCartReminderService _abandonedCartReminderService;

    public AbandonedCartReminderTask(
        ISettingService settingService,
        ILogger logger,
        IAbandonedCartReminderService abandonedCartReminderService)
    {
        _settingService = settingService;
        _logger = logger;
        _abandonedCartReminderService = abandonedCartReminderService;
    }

    public async Task ExecuteAsync()
    {
        Console.WriteLine("=== AbandonedCartReminderTask EXECUTED ===");

        var settings = await _settingService.LoadSettingAsync<AbandonedCartReminderSettings>();

        Console.WriteLine($"=== Enabled: {settings.Enabled}, SendAfterHours: {settings.SendAfterHours} ===");

        if (!settings.Enabled)
        {
            Console.WriteLine("=== Task skipped because plugin setting is disabled ===");
            await _logger.InformationAsync("AbandonedCartReminderTask skipped because plugin is disabled.");
            return;
        }

        await _abandonedCartReminderService.SendReminderEmailsAsync();

        await _logger.InformationAsync("AbandonedCartReminderTask completed successfully.");
    }
}