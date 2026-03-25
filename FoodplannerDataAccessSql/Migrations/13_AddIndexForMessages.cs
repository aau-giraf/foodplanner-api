using System.Data;
using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(13)]
public class AddIndexForMessages : Migration
{
    public override void Up()
    {
        
        Create.Index("idx_message_thread_date")
            .OnTable("message")
            .OnColumn("chat_thread_id").Ascending()
            .OnColumn("date").Ascending();
    }

    public override void Down()
    {
        Delete.Index("ix_message_thread_date").OnTable("message");
    }
}