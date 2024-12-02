using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations
{
    [Migration(4)]
    public class AddedColmnToTable : Migration
    {
        public override void Up()
        {
            Alter.Table("packed_ingredients")
                .AddColumn("order_number").AsInt32().NotNullable().WithDefaultValue(0);
        }

        public override void Down()
        {
            Delete.Column("order_number").FromTable("packed_ingredients");
        }
    }
}