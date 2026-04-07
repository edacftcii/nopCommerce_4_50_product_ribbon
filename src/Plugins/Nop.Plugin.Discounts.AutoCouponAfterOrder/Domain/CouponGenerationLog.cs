using System;
using Nop.Core;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Domain
{
    public class CouponGenerationLog : BaseEntity
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public int DiscountId { get; set; }

        public string CouponCode { get; set; }

        public decimal DiscountValue { get; set; }

        public int DiscountTypeId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? ExpirationDateUtc { get; set; }
    }
}