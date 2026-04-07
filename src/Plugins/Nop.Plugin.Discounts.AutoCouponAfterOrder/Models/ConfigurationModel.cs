using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Discounts.AutoCouponAfterOrder.Fields.Enabled")]
        public bool Enabled { get; set; }

        [NopResourceDisplayName("Plugins.Discounts.AutoCouponAfterOrder.Fields.DiscountType")]
        public int DiscountTypeId { get; set; }

        [NopResourceDisplayName("Plugins.Discounts.AutoCouponAfterOrder.Fields.DiscountValue")]
        public decimal DiscountValue { get; set; }

        [NopResourceDisplayName("Plugins.Discounts.AutoCouponAfterOrder.Fields.ValidDays")]
        public int ValidDays { get; set; }

        public IList<SelectListItem> AvailableDiscountTypes { get; set; } = new List<SelectListItem>();
    }
}