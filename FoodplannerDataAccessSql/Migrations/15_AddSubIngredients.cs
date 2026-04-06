using FluentMigrator;
using System.Data;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(15)]
public class AddSubIngredients : Migration
{
    public override void Up()
    {
        // Create subingredients table
        Create.Table("subingredients")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("user_id").AsInt32().NotNullable()
            .WithColumn("food_image_id").AsInt32().Nullable();

        // Add foreign key to users
        Create.ForeignKey("fk_subingredients_user_id")
            .FromTable("subingredients").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");

        // Add foreign key to food_image
        Create.ForeignKey("fk_subingredients_food_image_id")
            .FromTable("subingredients").ForeignColumn("food_image_id")
            .ToTable("food_image").PrimaryColumn("id");

        // Create junction table for many-to-many relationship
        Create.Table("subingredient_relation")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("ingredient_id").AsInt32().NotNullable()
            .WithColumn("subingredient_id").AsInt32().NotNullable()
            .WithColumn("order_number").AsInt32().NotNullable().WithDefaultValue(0);

        // Add foreign key to ingredients
        Create.ForeignKey("fk_subingredient_relation_ingredient_id")
            .FromTable("subingredient_relation").ForeignColumn("ingredient_id")
            .ToTable("ingredients").PrimaryColumn("id")
            .OnDelete(Rule.Cascade);

        // Add foreign key to subingredients
        Create.ForeignKey("fk_subingredient_relation_subingredient_id")
            .FromTable("subingredient_relation").ForeignColumn("subingredient_id")
            .ToTable("subingredients").PrimaryColumn("id")
            .OnDelete(Rule.Cascade);
    }

    public override void Down()
    {
        Delete.Table("subingredient_relation");
        Delete.Table("subingredients");
    }
}