using Nop.Plugin.Misc.AbandonedCartReminder.Models;

namespace Nop.Plugin.Misc.AbandonedCartReminder.Factories;

public class AbandonedCartReminderModelFactory : IAbandonedCartReminderModelFactory
{
    private readonly AbandonedCartReminderSettings _settings;

    public AbandonedCartReminderModelFactory(AbandonedCartReminderSettings settings)
    {
        _settings = settings;
    }

    public async Task<ConfigurationModel> PrepareConfigurationModelAsync()
    {
        var model = new ConfigurationModel
        {
            Enabled = _settings.Enabled,
            SendAfterHours = _settings.SendAfterHours
        };

        return await Task.FromResult(model);
    }
}