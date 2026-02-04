using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(14)]
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