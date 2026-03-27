using System.ComponentModel;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.ProductRibbon.Models
{
    public record ProductRibbonSearchModel : BaseSearchModel
    {
        [DisplayName("Plugin Enabled")]
        public bool PluginEnabled { get; set; }
    }
}