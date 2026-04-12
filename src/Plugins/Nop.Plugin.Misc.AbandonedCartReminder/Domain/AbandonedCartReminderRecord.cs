using Nop.Core;

namespace Nop.Plugin.Misc.AbandonedCartReminder.Domain;

public class AbandonedCartReminderRecord : BaseEntity
{
    public int CustomerId { get; set; }
    public int StoreId { get; set; }
    public DateTime CartLastUpdatedOnUtc { get; set; }
    public DateTime? ReminderSentOnUtc { get; set; }
    public bool IsRecovered { get; set; }
    public string CartSnapshot { get; set; }
}