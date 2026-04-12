using FluentMigrator;
using Nop.Data.Migrations;

namespace Nop.Plugin.Misc.AbandonedCartReminder.Data.Migrations;

[NopMigration("2026-04-11 13:00:00", "Add CartSnapshot column to AbandonedCartReminderRecord", MigrationProcessType.Update)]
public class AddCartSnapshotToAbandonedCartReminderRecord : Migration
{
    public override void Up()
    {
        Alter.Table("AbandonedCartReminderRecord")
            .AddColumn("CartSnapshot").AsString(int.MaxValue).Nullable();
    }

    public override void Down()
    {
        Delete.Column("CartSnapshot").FromTable("AbandonedCartReminderRecord");
    }
}