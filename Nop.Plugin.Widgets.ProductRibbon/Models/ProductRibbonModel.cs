using System.ComponentModel;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.ProductRibbon.Models
{
    public record ProductRibbonModel : BaseNopEntityModel
    {
        [DisplayName("Ribbon Text")]
        public string Name { get; set; }

        [DisplayName("Background Color")]
        public string BackgroundColor { get; set; }

        [DisplayName("Text Color")]
        public string TextColor { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }
    }
}