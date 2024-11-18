using FluentMigrator;
using Microsoft.Extensions.Primitives;

namespace FoodplannerDataAccessSql.Migrations;


[Migration(3)]
public class addedNewTables: Migration 
{
    public override void Up()
    {
        Create.Table("ingredients")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("user_id").AsInt32().NotNullable()
            .WithColumn("food_image_id").AsInt32().NotNullable();
            
        Create.ForeignKey("fk_user_id")
            .FromTable("ingredients").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");
        
        Create.ForeignKey("fk_food_image_id")
            .FromTable("ingredients").ForeignColumn("food_image_id")
            .ToTable("food_image").PrimaryColumn("id");

        Create.Table("meals")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("date").AsString(100).NotNullable()
            .WithColumn("user_id").AsInt32().NotNullable()
            .WithColumn("food_image_id").AsInt32().NotNullable();

        Create.ForeignKey("fk_user_id")
            .FromTable("meals").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");

        Create.ForeignKey("fk_food_image_id")
            .FromTable("meals").ForeignColumn("food_image_id")
            .ToTable("food_image").PrimaryColumn("id");

        Create.Table("packed_ingredients")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("meal_id").AsInt32().NotNullable()
            .WithColumn("ingredient_id").AsInt32().NotNullable();

        Create.ForeignKey("fk_meal_id")
            .FromTable("packed_ingredients").ForeignColumn("meal_id")
            .ToTable("meals").PrimaryColumn("id");

        Create.ForeignKey("fk_ingredient_id")
            .FromTable("packed_ingredients").ForeignColumn("ingredient_id")
            .ToTable("ingredients").PrimaryColumn("id");
    }

    public override void Down()
    {
        Delete.Table("ingredients");
        Delete.Table("meals");
        Delete.Table("packed_ingredients");
    }
}