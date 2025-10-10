using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(6)]
public class AddCascadeToForeignKeys : Migration
{
    public override void Up()
    {
        Execute.Sql(@"ALTER TABLE IF EXISTS packed_ingredients DROP CONSTRAINT IF EXISTS fk_packed_ingredients_ingredient_id;"
                    + @"ALTER TABLE packed_ingredients ADD CONSTRAINT fk_packed_ingredients_ingredient_id FOREIGN KEY (ingredient_id) REFERENCES ingredients(id) ON DELETE CASCADE;");
        
        Execute.Sql(@"ALTER TABLE IF EXISTS packed_ingredients DROP CONSTRAINT IF EXISTS fk_packed_ingredients_meal_id;"
                    + @"ALTER TABLE packed_ingredients ADD CONSTRAINT fk_packed_ingredients_meal_id FOREIGN KEY (meal_id) REFERENCES meals(id) ON DELETE CASCADE;");
    }

    public override void Down()
    {
        Execute.Sql(@"ALTER TABLE IF EXISTS packed_ingredients DROP CONSTRAINT IF EXISTS fk_packed_ingredients_ingredient_id;"
                    + @"ALTER TABLE packed_ingredients ADD CONSTRAINT fk_packed_ingredients_ingredient_id FOREIGN KEY (ingredient_id) REFERENCES ingredients(id)");
        
        Execute.Sql(@"ALTER TABLE IF EXISTS packed_ingredients DROP CONSTRAINT IF EXISTS fk_packed_ingredients_meal_id;"
                    + @"ALTER TABLE packed_ingredients ADD CONSTRAINT fk_packed_ingredients_meal_id FOREIGN KEY (meal_id) REFERENCES meals(id);");
    }
}
