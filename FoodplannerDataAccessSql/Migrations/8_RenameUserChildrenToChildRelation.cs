using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(8)]
public class RenameUserChildrenToChildRelation : Migration
{
    public override void Up()
    {
        // First, drop the existing foreign key constraints
        Delete.ForeignKey("fk_user_children_user_id").OnTable("user_children");
        Delete.ForeignKey("fk_user_children_child_id").OnTable("user_children");
        
        // Drop the primary key constraint
        Delete.PrimaryKey("pk_user_children").FromTable("user_children");
        
        // Rename the table from user_children to child_relation
        Rename.Table("user_children").To("child_relation");
        
        // Recreate the primary key constraint with the new table name
        Create.PrimaryKey("pk_child_relation")
            .OnTable("child_relation")
            .Columns("user_id", "child_id");
        
        // Recreate the foreign key constraints with the new table name
        Create.ForeignKey("fk_child_relation_user_id")
            .FromTable("child_relation").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");
        
        Create.ForeignKey("fk_child_relation_child_id")
            .FromTable("child_relation").ForeignColumn("child_id")
            .ToTable("children").PrimaryColumn("child_id");
    }

    public override void Down()
    {
        // Drop the foreign key constraints
        Delete.ForeignKey("fk_child_relation_user_id").OnTable("child_relation");
        Delete.ForeignKey("fk_child_relation_child_id").OnTable("child_relation");
        
        // Drop the primary key constraint
        Delete.PrimaryKey("pk_child_relation").FromTable("child_relation");
        
        // Rename the table back
        Rename.Table("child_relation").To("user_children");
        
        // Recreate the original primary key constraint
        Create.PrimaryKey("pk_user_children")
            .OnTable("user_children")
            .Columns("user_id", "child_id");
        
        // Recreate the original foreign key constraints
        Create.ForeignKey("fk_user_children_user_id")
            .FromTable("user_children").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");
        
        Create.ForeignKey("fk_user_children_child_id")
            .FromTable("user_children").ForeignColumn("child_id")
            .ToTable("children").PrimaryColumn("child_id");
    }
}