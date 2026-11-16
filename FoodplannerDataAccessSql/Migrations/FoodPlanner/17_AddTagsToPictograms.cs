using FluentMigrator;
using System.Data;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(17)]
[Tags("FoodPlanner")]
public class AddTagsToPictograms : Migration
{
    public override void Up()
    {
        // Create category table 
         Create.Table("categories")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("main").AsBoolean().NotNullable();

        // Create tags table 
        Create.Table("tags")
        .WithColumn("ingredient_id").AsInt32().NotNullable()
        .WithColumn("category_id").AsInt32().NotNullable();

        // Create tags primary key
        Create.PrimaryKey("pk_categories_ingredients")
        .OnTable("tags")
        .Columns("ingredient_id", "category_id");

        // Link the tag columns to ingredients table 
        Create.ForeignKey("fk_ingredient_id")
        .FromTable("tags").ForeignColumn("ingredient_id")
        .ToTable("ingredients").PrimaryColumn("id");

        // Link the tag columns to categories table 
        Create.ForeignKey("fk_category_id")
        .FromTable("tags").ForeignColumn("category_id")
        .ToTable("categories").PrimaryColumn("id");

        // Add column to ingredient table 
        Alter.Table("ingredients").AddColumn("is_stock_img").AsBoolean().NotNullable().WithDefaultValue(false);
    }

        public override void Down()
    {
        Delete.Table("tags");
        Delete.Table("categories");
        Delete.Column("is_stock_img");
    }
}