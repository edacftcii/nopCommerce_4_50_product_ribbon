using System;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Models
{
    public class PublicCouponNotificationModel
    {
        public bool HasCoupon { get; set; }

        public string CouponCode { get; set; }

        public decimal DiscountValue { get; set; }

        public string DiscountTypeText { get; set; }

        public DateTime? ExpirationDateUtc { get; set; }
    }
}