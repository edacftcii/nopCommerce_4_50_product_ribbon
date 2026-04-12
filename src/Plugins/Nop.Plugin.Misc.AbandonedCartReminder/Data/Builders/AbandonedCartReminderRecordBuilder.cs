using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.AbandonedCartReminder.Domain;

namespace Nop.Plugin.Misc.AbandonedCartReminder.Data.Builders;

public class AbandonedCartReminderRecordBuilder : NopEntityBuilder<AbandonedCartReminderRecord>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(AbandonedCartReminderRecord.CustomerId)).AsInt32().NotNullable()
            .WithColumn(nameof(AbandonedCartReminderRecord.StoreId)).AsInt32().NotNullable()
            .WithColumn(nameof(AbandonedCartReminderRecord.CartLastUpdatedOnUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(AbandonedCartReminderRecord.ReminderSentOnUtc)).AsDateTime2().Nullable()
            .WithColumn(nameof(AbandonedCartReminderRecord.IsRecovered)).AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(nameof(AbandonedCartReminderRecord.CartSnapshot)).AsString(int.MaxValue).Nullable();
    }
}