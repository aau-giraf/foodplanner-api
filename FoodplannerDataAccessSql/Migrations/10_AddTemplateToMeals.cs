using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(10)]
public class AddTemplateToMeals : Migration
{
    public override void Up()
    {
        Alter.Table("meals")
            .AddColumn("template").AsBoolean().NotNullable().WithDefaultValue(false);
    }

    public override void Down()
    {
        Delete.Column("template").FromTable("meals");
    }
}