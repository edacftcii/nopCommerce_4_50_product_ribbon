using Nop.Plugin.Misc.AbandonedCartReminder.Models;

namespace Nop.Plugin.Misc.AbandonedCartReminder.Factories;

public interface IAbandonedCartReminderModelFactory
{
    Task<ConfigurationModel> PrepareConfigurationModelAsync();
}