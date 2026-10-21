using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(3)]
[Tags("CoreMock")]
public class UpdateFoodImageTable : Migration
{
    public override void Up()
    {
        Delete.Column("image_id")
            .FromTable("food_image");

        Alter.Table("food_image")
            .AddColumn("image_data")
            .AsBinary()
            .Nullable();
    }

    public override void Down()
    {
        Delete.Column("image_data")
            .FromTable("food_image");

        Alter.Table("food_image")
            .AddColumn("image_id")
            .AsString()
            .Nullable();
    }
}