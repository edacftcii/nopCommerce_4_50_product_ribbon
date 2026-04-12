using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.AbandonedCartReminder;

public class AbandonedCartReminderPlugin : BasePlugin
{
    private readonly IRepository<MessageTemplate> _messageTemplateRepository;
    private readonly IScheduleTaskService _scheduleTaskService;
    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;

    public AbandonedCartReminderPlugin(
        IRepository<MessageTemplate> messageTemplateRepository,
        IScheduleTaskService scheduleTaskService,
        ISettingService settingService,
        IWebHelper webHelper)
    {
        _messageTemplateRepository = messageTemplateRepository;
        _scheduleTaskService = scheduleTaskService;
        _settingService = settingService;
        _webHelper = webHelper;
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new AbandonedCartReminderSettings
        {
            SendAfterHours = 24,
            Enabled = true
        });

        var existingTemplate = _messageTemplateRepository.Table
            .FirstOrDefault(x => x.Name == "AbandonedCartReminder");

        if (existingTemplate == null)
        {
            var template = new MessageTemplate
            {
                Name = "AbandonedCartReminder",
                Subject = "You left items in your cart",
                Body = @"<p>Hello,</p>
<p>You left items in your cart. Please come back and complete your order.</p>
<p>%Store.Name%</p>",
                IsActive = true
            };

            await _messageTemplateRepository.InsertAsync(template);
        }

        var task = (await _scheduleTaskService.GetAllTasksAsync())
            .FirstOrDefault(x => x.Type == AbandonedCartReminderDefaults.ScheduledTaskType);

        if (task == null)
        {
            await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
            {
                Name = "Abandoned cart reminder task",
                Seconds = 3600,
                Type = AbandonedCartReminderDefaults.ScheduledTaskType,
                Enabled = true,
                StopOnError = false
            });
        }

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        var task = (await _scheduleTaskService.GetAllTasksAsync())
            .FirstOrDefault(x => x.Type == AbandonedCartReminderDefaults.ScheduledTaskType);

        if (task != null)
            await _scheduleTaskService.DeleteTaskAsync(task);

        var template = _messageTemplateRepository.Table
            .FirstOrDefault(x => x.Name == "AbandonedCartReminder");

        if (template != null)
            await _messageTemplateRepository.DeleteAsync(template);

        await _settingService.DeleteSettingAsync<AbandonedCartReminderSettings>();

        await base.UninstallAsync();
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/AbandonedCartReminder/Configure";
    }
}