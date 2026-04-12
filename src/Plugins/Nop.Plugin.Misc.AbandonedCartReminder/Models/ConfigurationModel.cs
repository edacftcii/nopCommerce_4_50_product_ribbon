using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.AbandonedCartReminder.Models;

public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.Misc.AbandonedCartReminder.Fields.SendAfterHours")]
    public int SendAfterHours { get; set; }

    [NopResourceDisplayName("Plugins.Misc.AbandonedCartReminder.Fields.Enabled")]
    public bool Enabled { get; set; }
}