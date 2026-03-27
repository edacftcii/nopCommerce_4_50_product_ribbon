using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using ProductRibbonMappingEntity = Nop.Plugin.Widgets.ProductRibbon.Domain.ProductRibbonMapping;

namespace Nop.Plugin.Widgets.ProductRibbon.Data.Builders
{
    public class ProductRibbonMappingBuilder : NopEntityBuilder<ProductRibbonMappingEntity>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductRibbonMappingEntity.ProductId)).AsInt32().NotNullable()
                .WithColumn(nameof(ProductRibbonMappingEntity.ProductRibbonId)).AsInt32().NotNullable();
        }
    }
}