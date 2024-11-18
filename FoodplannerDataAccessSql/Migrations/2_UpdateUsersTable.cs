using FluentMigrator;
using Microsoft.Extensions.Primitives;

namespace FoodplannerDataAccessSql.Migrations;


[Migration(2)]
public class updateUsersTable: Migration 
{
    public override void Up()
    {
        // Step 1: Add a new temporary column
        Alter.Table("users")
            .AddColumn("role_approved_temp").AsBoolean().NotNullable().WithDefaultValue(false);

        // Step 2: Copy data from the old column to the new column
        Execute.Sql("UPDATE users SET role_approved_temp = (role_approved = 'true')");

        // Step 3: Drop the old column
        Delete.Column("role_approved").FromTable("users");

        // Step 4: Rename the new column to the old column name
        Rename.Column("role_approved_temp").OnTable("users").To("role_approved");

        // Other column modifications
        Alter.Column("id").OnTable("users").AsInt32().PrimaryKey().NotNullable();
        Execute.Sql("ALTER TABLE users ALTER COLUMN id SET DEFAULT nextval('users_id_seq')");
        Alter.Column("pincode").OnTable("users").AsString(100).Nullable();
        Alter.Table("users")
            .AddColumn("archived").AsBoolean().NotNullable().WithDefaultValue(false);
    }

    public override void Down()
    {
        // Step 1: Add a new temporary column
        Alter.Table("users")
            .AddColumn("role_approved_temp").AsString().NotNullable();

        // Step 2: Copy data from the old column to the new column
        Execute.Sql("UPDATE users SET role_approved_temp = CASE WHEN role_approved THEN 'true' ELSE 'false' END");

        // Step 3: Drop the old column
        Delete.Column("role_approved").FromTable("users");

        // Step 4: Rename the new column to the old column name
        Rename.Column("role_approved_temp").OnTable("users").To("role_approved");

        // Other column modifications
        Alter.Column("id").OnTable("users").AsString().NotNullable();
        Delete.PrimaryKey("PK_users").FromTable("users");
        Delete.Column("id").FromTable("users");
        Alter.Table("users")
            .AddColumn("id").AsInt32().PrimaryKey().NotNullable();
        
        Alter.Column("pincode").OnTable("users").AsString(100).NotNullable();
        Delete.Column("archived").FromTable("users");
    }
}