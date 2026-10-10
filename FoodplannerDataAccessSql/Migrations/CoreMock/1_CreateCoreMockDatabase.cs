using FluentMigrator;
using System.Data;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(1)]
[Tags("CoreMock")]
public class CreateCoreMockDatabase : Migration
{
public override void Up() 
{
    //Creates table "users"
    Create.Table("users")
        .WithColumn("id").AsInt32().PrimaryKey().Identity() //primary key that does auto-increment
        .WithColumn("first_name").AsString(30).NotNullable()
        .WithColumn("last_name").AsString(100).NotNullable()
        .WithColumn("email").AsString(100).NotNullable()
        .WithColumn("password").AsString(100).NotNullable()
        .WithColumn("role").AsString(30).NotNullable()
        .WithColumn("pincode").AsString(100).Nullable()
        .WithColumn("role_approved").AsBoolean().NotNullable()
        .WithColumn("archived").AsBoolean().NotNullable();
    
    //Creates table "classroom"
    Create.Table("classroom")
        .WithColumn("class_id").AsInt32().PrimaryKey().Identity() //primary key that auto-increments
        .WithColumn("class_name").AsString().NotNullable();
    
    //create table "Children"
    Create.Table("children")
        .WithColumn("child_id").AsInt32().PrimaryKey().NotNullable()
        .WithColumn("first_name").AsString(100).NotNullable()
        .WithColumn("last_name").AsString(100).NotNullable()
        .WithColumn("class_id").AsInt32().Nullable(); //foreign key pointing at classroom.class_id

    Create.Table("child_relation")
        .WithColumn("user_id").AsInt32().NotNullable()
        .WithColumn("child_id").AsInt32().NotNullable();
    
    Create.PrimaryKey("PK_child_relation")
        .OnTable("child_relation")
        .Columns("user_id", "child_id");

    //Creates table "food_image"
    Create.Table("food_image")
        .WithColumn("id").AsInt32().PrimaryKey().Identity() //primary key, auto incrementing
        .WithColumn("image_id").AsString().NotNullable()
        .WithColumn("user_id").AsInt32().NotNullable()
        .WithColumn("image_name").AsString().NotNullable()
        .WithColumn("image_file_type").AsString().NotNullable()
        .WithColumn("size").AsInt64().NotNullable()
        .WithColumn("image").AsCustom("bytea").NotNullable();

    Create.Table("message")
        .WithColumn("message_id").AsInt32().PrimaryKey().Identity()
        .WithColumn("chat_thread_id").AsInt32().NotNullable()
        .WithColumn("content").AsString(1000).NotNullable()
        .WithColumn("date").AsDateTime().NotNullable()
        .WithColumn("user_id").AsInt32().NotNullable()
        .WithColumn("archived").AsBoolean().NotNullable()
            .WithDefaultValue(false)
        .WithColumn("is_edited").AsBoolean().NotNullable()
            .WithDefaultValue(false);

    Create.Table("chat_thread")
        .WithColumn("chat_thread_id").AsInt32().PrimaryKey().Identity()
        .WithColumn("child_id").AsInt32().NotNullable();
    

    Create.Table("one_time_password")
        .WithColumn("code_id").AsInt32().PrimaryKey().Identity()
        .WithColumn("generated_by").AsInt32().NotNullable()
        .WithColumn("code").AsString(6).NotNullable().Unique()
        .WithColumn("created_on").AsDateTime().NotNullable()
            .WithDefault(SystemMethods.CurrentDateTime)
        .WithColumn("expires_on").AsDateTime().NotNullable()
        .WithColumn("used").AsBoolean().NotNullable()
            .WithDefaultValue(false)
        .WithColumn("used_by_user").AsInt32().Nullable()
        .WithColumn("child_user").AsInt32().Nullable();

    
    //foreign keys:

    //Children foriegn keys
    Create.ForeignKey("FK_children_users")
        .FromTable("children").ForeignColumn("child_id")
        .ToTable("users").PrimaryColumn("id");
    Create.ForeignKey("FK_children_classroom")
        .FromTable("children").ForeignColumn("class_id")
        .ToTable("classroom").PrimaryColumn("class_id");
    
    //child relation foreign keys:
    Create.ForeignKey("FK_child_relation_users")
        .FromTable("child_relation").ForeignColumn("user_id")
        .ToTable("users").PrimaryColumn("id");
    Create.ForeignKey("FK_child_relation_children")
        .FromTable("child_relation").ForeignColumn("child_id")
        .ToTable("children").PrimaryColumn("child_id");

    //Message foreign keys
    Create.ForeignKey("FK_message_chat_thread")
        .FromTable("message").ForeignColumn("chat_thread_id")
        .ToTable("chat_thread").PrimaryColumn("chat_thread_id");
    Create.ForeignKey("FK_message_users")
        .FromTable("message").ForeignColumn("user_id")
        .ToTable("users").PrimaryColumn("id");

    //chat thread foreign keys
    Create.ForeignKey("FK_chat_thread_children")
        .FromTable("chat_thread").ForeignColumn("child_id")
        .ToTable("children").PrimaryColumn("child_id");

    //image foreign keys
    Create.ForeignKey("FK_food_image_users")
        .FromTable("food_image").ForeignColumn("user_id")
        .ToTable("users").PrimaryColumn("id");

    //one time password foreign keys
    Create.ForeignKey("FK_otp_generated_by")
        .FromTable("one_time_password").ForeignColumn("generated_by")
        .ToTable("users").PrimaryColumn("id");
    Create.ForeignKey("FK_otp_used_by_user")
        .FromTable("one_time_password").ForeignColumn("used_by_user")
        .ToTable("users").PrimaryColumn("id");
    Create.ForeignKey("FK_otp_child_user")
        .FromTable("one_time_password").ForeignColumn("child_user")
        .ToTable("children").PrimaryColumn("child_id");
}
    /* Down() is a function from the FluentMigrator framework
    that defines how the migration is rolled back */
    public override void Down()
    {
        //Deletes the tables created by this migration
        Delete.Table("one_time_password");
        Delete.Table("message");
        Delete.Table("chat_thread");
        Delete.Table("child_relation");
        Delete.Table("food_image");
        Delete.Table("children");
        Delete.Table("classroom");
        Delete.Table("users");
    }
    
}