using FluentMigrator;
namespace FoodplannerDataAccessSql.Migrations
{
    [Migration(11)]
    public class AddOTPTable : Migration
    {
        public override void Up()
        {
            // create a table for storing one-time password codes
            Create.Table("one_time_password")
                .WithColumn("code_id").AsInt32().PrimaryKey().NotNullable().Identity()

                // The user who generated the one-time password codes
                .WithColumn("generated_by").AsInt32().NotNullable()
                    .ForeignKey("users", "id")

                // The six-character one-time password, each code must be unique
                .WithColumn("code").AsString(6).NotNullable().Unique()

                // Time when the code was created
                .WithColumn("created_on").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentDateTime)

                // Time after which the code is no longer valid
                .WithColumn("expires_on").AsDateTime().NotNullable()

                // Tracks whether the code has already been used
                .WithColumn("used").AsBoolean().NotNullable()
                    .WithDefaultValue(false)

                // User who used the code, if it has been used
                .WithColumn("used_by_user").AsInt32().Nullable()
                    .ForeignKey("users", "id")
                // Child user associated with the one-time password
                .WithColumn("child_user").AsInt32().Nullable()
                    .ForeignKey("users", "id");
                    
        }

        public override void Down()
        {
            // remove the one-time password table when rolling back the migration
            Delete.Table("one_time_password");
        }
    }
}
