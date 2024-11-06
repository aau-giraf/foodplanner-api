using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(20243210)]
public class CreateClassroomTable : Migration {
    public override void Up()
    {
        Create.Table("classroom")
            .WithColumn("class_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("class_name").AsString().NotNullable();

    }

    public override void Down()
    {
        Delete.Table("classroom");
    }
}