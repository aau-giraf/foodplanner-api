using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(5)]
public class FeedBackChatTables : Migration
{
        public override void Up()
        {
            Create.Table("chat_thread")
                .WithColumn("chat_thread_id").AsInt32().PrimaryKey().Identity()
                .WithColumn("child_id").AsInt32().NotNullable();
            
            Create.ForeignKey("fk_chat_thread_child_id")
                .FromTable("chat_thread").ForeignColumn("child_id")
                .ToTable("children").PrimaryColumn("child_id");
            
            Create.Table("message")
                .WithColumn("message_id").AsInt32().PrimaryKey().Identity()
                .WithColumn("chat_thread_id").AsInt32().NotNullable()
                .WithColumn("content").AsString(1000).NotNullable()
                .WithColumn("date").AsDateTime().NotNullable()
                .WithColumn("user_id").AsInt32().NotNullable()
                .WithColumn("archived").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("is_edited").AsBoolean().NotNullable().WithDefaultValue(false);
            
            Create.ForeignKey("fk_message_chat_thread_id")
                .FromTable("message").ForeignColumn("chat_thread_id")
                .ToTable("chat_thread").PrimaryColumn("chat_thread_id");
            
            Create.ForeignKey("fk_message_user_id")
                .FromTable("message").ForeignColumn("user_id")
                .ToTable("users").PrimaryColumn("id");
                
        }

    public override void Down()
    {
        Delete.Table("chat_thread");
        Delete.Table("message");
    }
}