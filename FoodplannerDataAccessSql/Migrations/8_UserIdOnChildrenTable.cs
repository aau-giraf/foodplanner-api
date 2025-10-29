using FluentMigrator;
using System.Data;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(8)]
public class UserIdOnChildrenTable : Migration
{
    public override void Up()
    {
        Rename.Column("child_id").OnTable("children").To("user_id");
        
        Create.ForeignKey("fk_children_child_id")
            .FromTable("children").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDeleteOrUpdate(Rule.Cascade);    
        
        Execute.Sql(@"
            UPDATE children c
            SET user_id = u.id
            FROM users u
            WHERE LOWER(c.first_name) = LOWER(u.first_name)
              AND LOWER(c.last_name) = LOWER(u.last_name);
        ");
    }

    public override void Down()
    {
        Delete.ForeignKey("fk_children_chi_id").OnTable("children");
    }
}