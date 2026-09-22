using System.Data;
using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(13)]
public class AddIndexForMessages : Migration
{
    public override void Up()
    {
        // Create an index on the chat_thread_id and date to improve
        // performence when retrieving messages from a chat thread by date
        Create.Index("idx_message_thread_date")
            .OnTable("message")
            .OnColumn("chat_thread_id").Ascending()
            .OnColumn("date").Ascending();
    }

    public override void Down()
    {
        // remove the message thread/date index when rolling back the migration
        Delete.Index("ix_message_thread_date").OnTable("message");
    }
}