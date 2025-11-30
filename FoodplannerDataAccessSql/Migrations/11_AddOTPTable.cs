using FluentMigrator;
namespace FoodplannerDataAccessSql.Migrations
{
    [Migration(11)]
    public class AddOTPTable : Migration
    {
        public override void Up()
        {
            Create.Table("one_time_password")
                .WithColumn("code_id").AsInt32().PrimaryKey().NotNullable().Identity()
                .WithColumn("generated_by").AsInt32().NotNullable()
                    .ForeignKey("users", "id")
                .WithColumn("code").AsString(6).NotNullable().Unique()
                .WithColumn("created_on").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("expires_on").AsDateTime().NotNullable()
                .WithColumn("used").AsBoolean().NotNullable()
                    .WithDefaultValue(false)
                .WithColumn("used_by_user").AsInt32().Nullable()
                    .ForeignKey("users", "id");
        }

        public override void Down()
        {
            Delete.Table("one_time_password");
        }
    }
}
