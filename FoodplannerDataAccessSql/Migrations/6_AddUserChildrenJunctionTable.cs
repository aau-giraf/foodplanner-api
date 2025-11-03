using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(6)]
public class AddUserChildrenJunctionTable : Migration
{
    public override void Up()
    {
        Delete.ForeignKey("fk_children_parent_id").OnTable("children");
        
        Delete.Column("parent_id").FromTable("children");
        
        Create.Table("user_children")
            .WithColumn("user_id").AsInt32().NotNullable()
            .WithColumn("child_id").AsInt32().NotNullable();
        
        Create.PrimaryKey("pk_user_children")
            .OnTable("user_children")
            .Columns("user_id", "child_id");
        
        Create.ForeignKey("fk_user_children_user_id")
            .FromTable("user_children").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");
        
        Create.ForeignKey("fk_user_children_child_id")
            .FromTable("user_children").ForeignColumn("child_id")
            .ToTable("children").PrimaryColumn("child_id");
    }

    public override void Down()
    {
        Delete.Table("user_children");
        
        Alter.Table("children")
            .AddColumn("parent_id").AsInt32().NotNullable();
        
        Create.ForeignKey("fk_children_parent_id")
            .FromTable("children").ForeignColumn("parent_id")
            .ToTable("users").PrimaryColumn("id");
    }
}

