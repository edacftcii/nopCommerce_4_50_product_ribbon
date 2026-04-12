using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.AbandonedCartReminder;

public class AbandonedCartReminderSettings : ISettings
{
    public int SendAfterHours { get; set; }
    public bool Enabled { get; set; }
}