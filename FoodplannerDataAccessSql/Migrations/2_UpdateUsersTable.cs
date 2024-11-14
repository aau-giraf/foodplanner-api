using FluentMigrator;
using Microsoft.Extensions.Primitives;

namespace FoodplannerDataAccessSql.Migrations;


[Migration(2)]
public class updateUsersTable: Migration 
{
    public override void Up()
    {
        Alter.Column("role_approved").OnTable("users").AsBoolean().NotNullable();
        Alter.Column("id").OnTable("users").AsInt32().PrimaryKey().NotNullable().Identity();
        Alter.Column("pincode").OnTable("users").AsString(100).Nullable();
        Alter.Table("users")
            .AddColumn("archived").AsBoolean().NotNullable().WithDefaultValue(false);
    }

    public override void Down()
    {
        Alter.Column("role_approved").OnTable("users").AsString().NotNullable();
        
        Delete.PrimaryKey("PK_users").FromTable("users");
        Delete.Column("id").FromTable("users");
        Alter.Table("users")
            .AddColumn("id").AsInt32().PrimaryKey().NotNullable();
        
        Alter.Column("pincode").OnTable("users").AsString(100).NotNullable();
        Delete.Column("archived").FromTable("users");
    }
}