using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Models
{
    public record CouponGenerationLogModel : BaseNopEntityModel
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public int DiscountId { get; set; }

        public string CouponCode { get; set; }

        public decimal DiscountValue { get; set; }

        public string DiscountTypeName { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? ExpirationDateUtc { get; set; }
    }
}