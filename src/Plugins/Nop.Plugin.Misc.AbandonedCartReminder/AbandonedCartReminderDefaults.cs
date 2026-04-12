namespace Nop.Plugin.Misc.AbandonedCartReminder;

public static class AbandonedCartReminderDefaults
{
    public const string SystemName = "Misc.AbandonedCartReminder";

    public const string ReminderMessageTemplateSystemName =
        "AbandonedCartReminder.CustomerNotification";

    public const string ScheduledTaskType =
        "Nop.Plugin.Misc.AbandonedCartReminder.Tasks.AbandonedCartReminderTask, Nop.Plugin.Misc.AbandonedCartReminder";
}