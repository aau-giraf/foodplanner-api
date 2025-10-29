using FluentMigrator;
using System.Data;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(8)]
public class UserIdOnChildrenTable : Migration
{
    public override void Up()
    {
        Alter.Column("class_id").OnTable("children").AsInt32().Nullable();
        
        Create.ForeignKey("fk_children_child_id")
            .FromTable("children").ForeignColumn("child_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDeleteOrUpdate(Rule.Cascade);    
        
        Execute.Sql(@"
            UPDATE children c
            SET child_id = u.id
            FROM users u
            WHERE LOWER(c.first_name) = LOWER(u.first_name)
              AND LOWER(c.last_name) = LOWER(u.last_name);
        ");
    }

    public override void Down()
    {
        Alter.Column("class_id").OnTable("children").AsInt32().NotNullable();
        
        Delete.ForeignKey("fk_children_child_id").OnTable("children");r
    }
}