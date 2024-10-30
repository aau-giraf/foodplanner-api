using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;


[Migration(20243010)]
public class create_food_image_table : Migration {
    
    public override void Up() {
        Create.Table("food_image")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("image_id").AsString().NotNullable()
            .WithColumn("user_id").AsInt32().NotNullable()
            .WithColumn("image_name").AsString().NotNullable()
            .WithColumn("image_file_type").AsString().NotNullable()
            .WithColumn("size").AsInt64().NotNullable();
    }

    public override void Down() {
        Delete.Table("food_image");
    }
    
}