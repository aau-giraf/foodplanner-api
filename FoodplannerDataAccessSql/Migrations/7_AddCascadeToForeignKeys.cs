using FluentMigrator;
using System.Data;
    
    namespace FoodplannerDataAccessSql.Migrations;
    
    [Migration(7)]
    public class AddCascadeToForeignKeys : Migration
    {
        public override void Up()
        {
            // Recreate the foreign key with cascading delete behavior
            Delete.ForeignKey("fk_packed_ingredients_ingredient_id").OnTable("packed_ingredients");
    
            Create.ForeignKey("fk_packed_ingredients_ingredient_id")
                .FromTable("packed_ingredients").ForeignColumn("ingredient_id")
                .ToTable("ingredients").PrimaryColumn("id")
                .OnDelete(Rule.Cascade);
        }
    
        public override void Down()
        {
            // Restore the foreign key without cascading delete behavior
            Delete.ForeignKey("fk_packed_ingredients_ingredient_id").OnTable("packed_ingredients");
    
            Create.ForeignKey("fk_packed_ingredients_ingredient_id")
                .FromTable("packed_ingredients").ForeignColumn("ingredient_id")
                .ToTable("ingredients").PrimaryColumn("id");
        }
    }