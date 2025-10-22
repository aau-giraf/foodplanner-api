using FluentMigrator;
using System.Data;
    
    namespace FoodplannerDataAccessSql.Migrations;
    
    [Migration(7)]
    public class ChildIdForeignKeyUserid : Migration
    {
        public override void Up()
        {
            Delete.Column("child_id").FromTable("children");
    
            Create.ForeignKey("fk_users_child_id")
                .FromTable("users").ForeignColumn("id")
                .ToTable("children").PrimaryColumn("child_id")
                .OnDelete(Rule.Cascade);
        }
    
        public override void Down()
        {
            Delete.ForeignKey("fk_users_child_id").OnTable("users");

            Create.Column("child_id")
                .OnTable("children")
                .AsInt32()
                .PrimaryKey()
                .Identity()
                .NotNullable();
        }
    }