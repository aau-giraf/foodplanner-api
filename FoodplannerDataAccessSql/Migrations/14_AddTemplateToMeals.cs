using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(14)]
public class AddTemplateToMeals : Migration
{
    public override void Up()
    {
        // Add "template" column to the "meals" table. Existing meals are set by default to false
        Alter.Table("meals")
            .AddColumn("template").AsBoolean().NotNullable().WithDefaultValue(false);
    }

    public override void Down()
    {
        // Remove "template" column when rolling back the migration
        Delete.Column("template").FromTable("meals");
    }
}