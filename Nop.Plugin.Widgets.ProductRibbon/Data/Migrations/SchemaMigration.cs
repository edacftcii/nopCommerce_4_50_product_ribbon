using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using ProductRibbonEntity = Nop.Plugin.Widgets.ProductRibbon.Domain.ProductRibbon;
using ProductRibbonMappingEntity = Nop.Plugin.Widgets.ProductRibbon.Domain.ProductRibbonMapping;

namespace Nop.Plugin.Widgets.ProductRibbon.Data.Migrations
{
    [NopMigration("2026/03/27 00:00:00:0000000", "Widgets.ProductRibbon base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<ProductRibbonEntity>();
            Create.TableFor<ProductRibbonMappingEntity>();
        }
    }
}