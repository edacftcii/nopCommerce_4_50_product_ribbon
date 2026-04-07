using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Domain;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Data.Migrations
{
    [NopMigration("2026/03/30 20:00:00:0000000", "Discounts.AutoCouponAfterOrder base schema", MigrationProcessType.Installation)]
    public class AutoCouponSchemaMigration : MigrationBase
    {
        public override void Up()
        {
            if (!Schema.Table(nameof(CouponGenerationLog)).Exists())
                Create.TableFor<CouponGenerationLog>();
        }

public override void Down()
{
    // Cleanup uninstall sırasında plugin içinden yapılacak.
}
    }
}