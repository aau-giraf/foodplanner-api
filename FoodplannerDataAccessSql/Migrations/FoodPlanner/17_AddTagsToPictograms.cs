using FluentMigrator;
using System.Data;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(17)]
[Tags("FoodPlanner")]
public class AddTagsToPictograms : Migration
{
    public override void Up()
    {
         Create.Table("category")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("main").AsBoolean().NotNullable();


        Create.ForeignKey("fk_category_id")
        .FromTable("category").ForeignColumn()
    }

        public override void Down()
    {
        Delete.Table("category_relation");
        Delete.Table("category");
    }
}