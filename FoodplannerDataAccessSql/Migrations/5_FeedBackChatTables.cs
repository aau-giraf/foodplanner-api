using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;
//fifth iteration of the database
[Migration(5)]
public class FeedBackChatTables : Migration
{
        //changes when migration is run
        public override void Up()
        {
            //create table "chat_thread"
            Create.Table("chat_thread")
                .WithColumn("chat_thread_id").AsInt32().PrimaryKey().Identity() //primary key that auto-increments
                .WithColumn("child_id").AsInt32().NotNullable(); //foreign key to children.child_id
            
            //create foreignkey from "chat_thread.child_id" that points to "children.child_id"
            Create.ForeignKey("fk_chat_thread_child_id")
                .FromTable("chat_thread").ForeignColumn("child_id")
                .ToTable("children").PrimaryColumn("child_id");

            //create table "message"           
            Create.Table("message")
                .WithColumn("message_id").AsInt32().PrimaryKey().Identity() //primary key that auto-increments
                .WithColumn("chat_thread_id").AsInt32().NotNullable() //foreign key to chat_thread.chat_thread_id
                .WithColumn("content").AsString(1000).NotNullable()
                .WithColumn("date").AsDateTime().NotNullable()
                .WithColumn("user_id").AsInt32().NotNullable() //foreign key to user.id
                .WithColumn("archived").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("is_edited").AsBoolean().NotNullable().WithDefaultValue(false);
            
            //create foreign key from "message.chat_thread_id" that points to "chat_thread.chat_thread_id"
            Create.ForeignKey("fk_message_chat_thread_id")
                .FromTable("message").ForeignColumn("chat_thread_id")
                .ToTable("chat_thread").PrimaryColumn("chat_thread_id");
            
            //creates foreign key from "message.user_id" that points to "users.id"
            Create.ForeignKey("fk_message_user_id")
                .FromTable("message").ForeignColumn("user_id")
                .ToTable("users").PrimaryColumn("id");
                
        }

    //removal of changes when migrations are rolled back
    public override void Down()
    {   
        //deletes the created tables
        Delete.Table("chat_thread");
        Delete.Table("message");
    }
}