using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using ProductRibbonEntity = Nop.Plugin.Widgets.ProductRibbon.Domain.ProductRibbon;

namespace Nop.Plugin.Widgets.ProductRibbon.Data.Builders
{
    public class ProductRibbonBuilder : NopEntityBuilder<ProductRibbonEntity>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductRibbonEntity.Name)).AsString(200).NotNullable()
                .WithColumn(nameof(ProductRibbonEntity.BackgroundColor)).AsString(20).NotNullable()
                .WithColumn(nameof(ProductRibbonEntity.TextColor)).AsString(20).NotNullable()
                .WithColumn(nameof(ProductRibbonEntity.IsActive)).AsBoolean().NotNullable();
        }
    }
}