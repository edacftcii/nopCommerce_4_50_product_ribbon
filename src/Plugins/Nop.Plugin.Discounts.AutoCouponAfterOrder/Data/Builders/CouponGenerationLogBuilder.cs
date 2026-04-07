using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Domain;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Data.Builders
{
    public class CouponGenerationLogBuilder : NopEntityBuilder<CouponGenerationLog>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CouponGenerationLog.OrderId)).AsInt32().NotNullable()
                .WithColumn(nameof(CouponGenerationLog.CustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(CouponGenerationLog.DiscountId)).AsInt32().NotNullable()
                .WithColumn(nameof(CouponGenerationLog.CouponCode)).AsString(400).NotNullable()
                .WithColumn(nameof(CouponGenerationLog.DiscountValue)).AsDecimal(18, 4).NotNullable()
                .WithColumn(nameof(CouponGenerationLog.DiscountTypeId)).AsInt32().NotNullable()
                .WithColumn(nameof(CouponGenerationLog.CreatedOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(CouponGenerationLog.ExpirationDateUtc)).AsDateTime2().Nullable();
        }
    }
}