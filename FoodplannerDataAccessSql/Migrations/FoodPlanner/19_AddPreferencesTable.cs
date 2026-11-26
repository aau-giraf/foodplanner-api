using FluentMigrator;
using System.Data;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(19)]
[Tags("FoodPlanner")]
public class AddPreferencesTable : Migration
{
    public override void Up()
    {
        // Create preference table 
         Create.Table("preferences")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("child_id").AsInt32().NotNullable()
            .WithColumn("food_image_id").AsInt32().Nullable()
            .WithColumn("food_name").AsString(100).NotNullable()
            .WithColumn("rating").AsString(100).NotNullable()
            .WithColumn("date").AsDateTime().NotNullable();
        }

        public override void Down()
    {
        Delete.Table("preferences");
    }
}