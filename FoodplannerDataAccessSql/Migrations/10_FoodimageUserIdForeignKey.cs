using System.Data;
using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(10)]
public class FoodimageUserIdForeignKey : Migration
{
    public override void Up()
    {
        Create.ForeignKey("fk_food_image_user_id")
            .FromTable("food_image").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDeleteOrUpdate(Rule.Cascade);
    }

    public override void Down()
    {
        Delete.ForeignKey("fk_food_image_user_id").OnTable("food_image");
    }
}