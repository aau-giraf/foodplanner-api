using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(18)]
[Tags("FoodPlanner")]
public class MealsDateTime: Migration
{
    public override void Up()
    {
        Execute.Sql("""
            ALTER TABLE meals
            ALTER COLUMN date TYPE DATE
            USING date::DATE;
        """);
        Console.Write("test");
    }

    public override void Down()
    {
        Execute.Sql("""
            ALTER TABLE meals
            ALTER COLUMN date TYPE VARCHAR(100)
            USING date::VARCHAR(100);
        """);
    }
}