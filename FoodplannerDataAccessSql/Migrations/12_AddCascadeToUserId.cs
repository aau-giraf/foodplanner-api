using System.Data;
using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(12)]
public class AddCascadeToUserId : Migration
{
    public override void Up()
    {
        const string userIdFkName = "fk_child_relation_user_id";

        Delete.ForeignKey(userIdFkName).OnTable("child_relation");

        Create.ForeignKey(userIdFkName)
            .FromTable("child_relation").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(Rule.Cascade)
            .OnUpdate(Rule.Cascade);
        
        const string childIdFkName = "fk_child_relation_child_id";
        Delete.ForeignKey(childIdFkName).OnTable("child_relation");
        
        Create.ForeignKey(childIdFkName)
            .FromTable("child_relation").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(Rule.Cascade)
            .OnUpdate(Rule.Cascade);
    }

    public override void Down()
    {
        const string userIdFkName = "fk_child_relation_user_id";
        Delete.ForeignKey(userIdFkName).OnTable("child_relation");

        Create.ForeignKey(userIdFkName)
            .FromTable("child_relation").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");
        
        const string childIdFkName = "fk_child_relation_child_id";
        Delete.ForeignKey(childIdFkName).OnTable("child_relation");

        Create.ForeignKey(childIdFkName)
            .FromTable("child_relation").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");
    }
}