using System.Data;
using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(10)]
public class FoodimageUserIdForeignKey : Migration
{
    public override void Up()
    {

        /* Create a foreign key linking food_image.user_id to users.id
        Changes to the referenced user are cascaded to the related food images*/
        Create.ForeignKey("fk_food_image_user_id")
            .FromTable("food_image").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDeleteOrUpdate(Rule.Cascade);
    }

    public override void Down()
    {
        // Remmove the foreign key between food_image and users
        Delete.ForeignKey("fk_food_image_user_id").OnTable("food_image");
    }
}