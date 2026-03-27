using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.ProductRibbon.Models
{
    public record ProductRibbonMappingModel : BaseNopModel
    {
        [DisplayName("Product")]
        public int ProductId { get; set; }

        [DisplayName("Ribbon")]
        public int ProductRibbonId { get; set; }

        public IList<SelectListItem> AvailableProducts { get; set; } = new List<SelectListItem>();
        public IList<SelectListItem> AvailableRibbons { get; set; } = new List<SelectListItem>();

        public IList<ProductRibbonMappingListItemModel> ExistingMappings { get; set; } = new List<ProductRibbonMappingListItemModel>();
    }

    public record ProductRibbonMappingListItemModel : BaseNopEntityModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public int ProductRibbonId { get; set; }
        public string RibbonName { get; set; }
    }
}