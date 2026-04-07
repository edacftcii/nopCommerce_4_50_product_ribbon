using Nop.Core.Configuration;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder
{
    public class AutoCouponSettings : ISettings
    {
        public bool Enabled { get; set; }
        public int DiscountTypeId { get; set; }
        public decimal DiscountValue { get; set; }
        public int ValidDays { get; set; }
    }
}