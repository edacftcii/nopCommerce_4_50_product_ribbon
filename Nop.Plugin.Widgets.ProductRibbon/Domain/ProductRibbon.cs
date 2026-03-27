using Nop.Core;

namespace Nop.Plugin.Widgets.ProductRibbon.Domain
{
    public class ProductRibbon : BaseEntity
    {
        public string Name { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
        public bool IsActive { get; set; }
    }
}