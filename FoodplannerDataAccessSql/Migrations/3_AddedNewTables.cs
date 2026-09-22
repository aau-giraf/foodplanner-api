using FluentMigrator;
using Microsoft.Extensions.Primitives;

namespace FoodplannerDataAccessSql.Migrations;

//third iteration of the database
[Migration(3)]
public class addedNewTables : Migration
{
    //changes when migration is run
    public override void Up()
    {
        //Creates table "meals"
        Create.Table("meals")
            .WithColumn("id").AsInt32().PrimaryKey().Identity() //primary key that auto-increments
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("date").AsString(100).NotNullable()
            .WithColumn("food_image_id").AsInt32().Nullable() //Foreign key to food_image.id
            .WithColumn("user_id").AsInt32().NotNullable(); //Foreign key to users.id

        //Creates a foreign key from "meals.user_id" that points to "users.id"
        Create.ForeignKey("fk_meals_user_id")
            .FromTable("meals").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");

        //Creates a foreign key from "meals.food_image_id" that points to "food_image.id"
        Create.ForeignKey("fk_meals_food_image_id")
            .FromTable("meals").ForeignColumn("food_image_id")
            .ToTable("food_image").PrimaryColumn("id");

        //Creates table "ingredients"
        Create.Table("ingredients")
            .WithColumn("id").AsInt32().PrimaryKey().Identity() //primary key that auto-increments
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("food_image_id").AsInt32().Nullable() //Foreign key to food_image.id
            .WithColumn("user_id").AsInt32().NotNullable(); //Foreign key to user.id

        //Creates foreign key from "ingredients.user_id" that points to "users.id"
        Create.ForeignKey("fk_ingredients_user_id")
            .FromTable("ingredients").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");

        //Creates foreign key from "ingredients.food_image_id" that points to "food_image.id"
        Create.ForeignKey("fk_ingredients_food_image_id")
            .FromTable("ingredients").ForeignColumn("food_image_id")
            .ToTable("food_image").PrimaryColumn("id");

        //Create table "packed_ingredients"
        Create.Table("packed_ingredients")
            .WithColumn("id").AsInt32().PrimaryKey().Identity() //primary key that auto-increment
            .WithColumn("meal_id").AsInt32().NotNullable() //Foreign key to meals.id
            .WithColumn("ingredient_id").AsInt32().NotNullable(); //Foreign key to ingredients.id

        //create foreign key from "packed_ingredients.meal_id" that points to "meals.id"
        Create.ForeignKey("fk_packed_ingredients_food_image_id")
            .FromTable("packed_ingredients").ForeignColumn("meal_id")
            .ToTable("meals").PrimaryColumn("id");

        //create foreign key from "packed_ingredients.ingredient_id" that points to "ingredients.id"
        Create.ForeignKey("fk_packed_ingredients_ingredient_id")
            .FromTable("packed_ingredients").ForeignColumn("ingredient_id")
            .ToTable("ingredients").PrimaryColumn("id");
    }

    //removal of changes when migrations are rolled back
    public override void Down()
    {
        //deletes the tables created in this migration
        Delete.Table("meals");
        Delete.Table("ingredients");
        Delete.Table("packed_ignredients");
    }
}