using Nop.Core;

namespace Nop.Plugin.Widgets.ProductRibbon.Domain
{
    public class ProductRibbonMapping : BaseEntity
    {
        public int ProductId { get; set; }
        public int ProductRibbonId { get; set; }
    }
}