using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(6)]
public class AddUserChildrenJunctionTable : Migration
{
    public override void Up()
    {
        // Remove the old direct relationship between children and their parent
        Delete.ForeignKey("fk_children_parent_id").OnTable("children");
        
        Delete.Column("parent_id").FromTable("children");

        // Create a table for relationships between users and children
        Create.Table("user_children")
            .WithColumn("user_id").AsInt32().NotNullable()
            .WithColumn("child_id").AsInt32().NotNullable();

        /* Use user_id and child_id together as the primary key
        this prevents duplicate user-child relationships*/
        Create.PrimaryKey("pk_user_children")
            .OnTable("user_children")
            .Columns("user_id", "child_id");

        // Link user_id to corresponding user
        Create.ForeignKey("fk_user_children_user_id")
            .FromTable("user_children").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");

        // Link child_id to the corresponding child
        Create.ForeignKey("fk_user_children_child_id")
            .FromTable("user_children").ForeignColumn("child_id")
            .ToTable("children").PrimaryColumn("child_id");
    }

    public override void Down()
    {
        // Remove the user-child junction table
        Delete.Table("user_children");
        
        // restore parent_id on the children table
        Alter.Table("children")
            .AddColumn("parent_id").AsInt32().NotNullable();
        
        // Restore the original relationship between children and users
        Create.ForeignKey("fk_children_parent_id")
            .FromTable("children").ForeignColumn("parent_id")
            .ToTable("users").PrimaryColumn("id");
    }
}

