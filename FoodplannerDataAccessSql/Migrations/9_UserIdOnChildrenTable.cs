using FluentMigrator;
using System.Data;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(9)]
public class UserIdOnChildrenTable : Migration
{
    public override void Up()
    {
        // Allow children to exist without being assigned to a class
        Alter.Column("class_id").OnTable("children").AsInt32().Nullable();
        
        /*create a foreign key linking children.child_id to user.id
        with cascading delete and update behavior*/
        Create.ForeignKey("fk_children_child_id")
            .FromTable("children").ForeignColumn("child_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDeleteOrUpdate(Rule.Cascade);    
        /* Match children to their corresponding users by first and last name
        only when the child's name is unique.*/
        Execute.Sql(@"
            UPDATE children c
            SET child_id = u.id
            FROM users u
            WHERE LOWER(c.first_name) = LOWER(u.first_name)
              AND LOWER(c.last_name) = LOWER(u.last_name)
              AND (
                SELECT COUNT(*) FROM children c2
                WHERE LOWER(c2.first_name) = LOWER(c.first_name)
                  AND LOWER(c2.last_name) = LOWER(c.last_name)
              ) = 1;
        ");
    }

    public override void Down()
    {
        // Restore class_id as a required column
        Alter.Column("class_id").OnTable("children").AsInt32().NotNullable();
        
        // Remove the foreign key between children.child_id and users.id
        Delete.ForeignKey("fk_children_child_id").OnTable("children");
    }
}