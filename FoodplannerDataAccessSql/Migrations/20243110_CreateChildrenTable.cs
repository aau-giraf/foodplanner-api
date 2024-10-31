using System.Data;
using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(20243110)]
public class CreateChildrenTable : Migration {
    public override void Up()
    {
        Create.Table("children")
            .WithColumn("child_id").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("first_name").AsString(100).NotNullable()
            .WithColumn("last_name").AsString(100).NotNullable()
            .WithColumn("parent_id").AsInt32().NotNullable()
            .WithColumn("class_id").AsInt32().NotNullable();
        
        Create.ForeignKey("fk_children_parent_id")
            .FromTable("children").ForeignColumn("parent_id")
            .ToTable("users").PrimaryColumn("id");
        
        Create.ForeignKey("fk_children_class_id")
            .FromTable("children").ForeignColumn("class_id")
            .ToTable("classroom").PrimaryColumn("class_id");
    }

    public override void Down()
    {
        Delete.Table("children");
    }
    
}