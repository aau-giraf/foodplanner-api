using FluentMigrator;
using System.Data;

namespace FoodplannerDataAccessSql.Migrations;

[Migration(16)]
[Tags("FoodPlanner")]
public class RestructureFoodplanner : Migration
{
    public override void Up()
    {
        // Remove foreign keys from meals referencing Core-owned tables.
        Delete.ForeignKey("fk_meals_food_image_id").OnTable("meals");
        Delete.ForeignKey("fk_meals_user_id").OnTable("meals");

        // Remove foreign keys from ingredients referencing Core-owned tables.
        Delete.ForeignKey("fk_ingredients_food_image_id").OnTable("ingredients");
        Delete.ForeignKey("fk_ingredients_user_id").OnTable("ingredients");

        // Remove foreign keys from subingredients referencing Core-owned tables.
        Delete.ForeignKey("fk_subingredients_food_image_id").OnTable("subingredients");
        Delete.ForeignKey("fk_subingredients_user_id").OnTable("subingredients");

        // Remove Core Tables
        Delete.Table("one_time_password");
        Delete.Table("message");
        Delete.Table("chat_thread");
        Delete.Table("child_relation");
        Delete.Table("children");
        Delete.Table("classroom");
        Delete.Table("food_image");
        Delete.Table("users");

        // Rename the image reference in ingredients and meals.
        Rename.Column("food_image_id")
            .OnTable("ingredients")
            .To("image_id");
        Rename.Column("food_image_id")
            .OnTable("meals")
            .To("image_id");

        // Rename packed_ingredients to packed.
        Rename.Table("packed_ingredients").To("packed");

        // Rename order_number to order_no.
        Rename.Column("order_number")
            .OnTable("packed")
            .To("order_no");

        // Remove the old primary key.
        Delete.PrimaryKey("PK_packed_ingredients")
            .FromTable("packed");

        // Remove the old ID column.
        Delete.Column("id").FromTable("packed");

        // Create a composite primary key.
        Create.PrimaryKey("pk_packed")
            .OnTable("packed")
            .Columns("meal_id", "ingredient_id");


        // Create a temporary mapping between old subingredient IDs
        // and the newly generated ingredient IDs.
        Execute.Sql("""
            CREATE TEMP TABLE subingredient_id_map (
                old_id INTEGER PRIMARY KEY,
                new_id INTEGER NOT NULL UNIQUE
            ) ON COMMIT DROP;
            """);

        // Synchronize the ingredient sequence with existing IDs.
        // This prevents generated IDs from colliding with old rows.
        Execute.Sql("""
            SELECT setval(
                pg_get_serial_sequence('ingredients', 'id'),
                GREATEST(COALESCE(MAX(id), 0), 1),
                MAX(id) IS NOT NULL
            )
            FROM ingredients;
            """);

        // Move existing subingredients into the ingredients table.
        // Store the old-to-new ID mapping for their relations.
        Execute.Sql("""
            DO $$
            DECLARE
                item RECORD;
                new_ingredient_id INTEGER;
            BEGIN
                FOR item IN
                    SELECT id, name, user_id, food_image_id
                    FROM subingredients
                    ORDER BY id
                LOOP
                    INSERT INTO ingredients (
                        name,
                        user_id,
                        image_id
                    )
                    VALUES (
                        item.name,
                        item.user_id,
                        item.food_image_id
                    )
                    RETURNING id INTO new_ingredient_id;

                    INSERT INTO subingredient_id_map (
                        old_id,
                        new_id
                    )
                    VALUES (
                        item.id,
                        new_ingredient_id
                    );
                END LOOP;
            END $$;
            """);

        // Create the new subingredient relation table.
        Create.Table("subingredient")
            .WithColumn("parent_id").AsInt32().NotNullable()
            .WithColumn("child_id").AsInt32().NotNullable()
            .WithColumn("order_no").AsInt32().NotNullable();

        // Create a composite primary key.
        Create.PrimaryKey("pk_subingredient")
            .OnTable("subingredient")
            .Columns("parent_id", "child_id");

        // Both parent and child reference the ingredients table.
        Create.ForeignKey("fk_subingredient_parent")
            .FromTable("subingredient").ForeignColumn("parent_id")
            .ToTable("ingredients").PrimaryColumn("id");

        Create.ForeignKey("fk_subingredient_child")
            .FromTable("subingredient").ForeignColumn("child_id")
            .ToTable("ingredients").PrimaryColumn("id");

        // Transfer the old relations using the ID mapping.
        Execute.Sql("""
            INSERT INTO subingredient (
                parent_id,
                child_id,
                order_no
            )
            SELECT
                r.ingredient_id,
                m.new_id,
                r.order_number
            FROM subingredient_relation r
            JOIN subingredient_id_map m
                ON m.old_id = r.subingredient_id;
            """);

        // Remove the obsolete relation table first.
        Delete.Table("subingredient_relation");

        // Remove the old subingredients table.
        Delete.Table("subingredients");


    }

    public override void Down()
    {


        // Remove the new subingredient relation table.
        Delete.Table("subingredient");

        // Recreate the original subingredients table.
        Create.Table("subingredients")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString().NotNullable()
            .WithColumn("user_id").AsInt32().NotNullable()
            .WithColumn("food_image_id").AsInt32().Nullable();

        // Recreate the original subingredient_relation table.
        Create.Table("subingredient_relation")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("ingredient_id").AsInt32().NotNullable()
            .WithColumn("subingredient_id").AsInt32().NotNullable()
            .WithColumn("order_number").AsInt32().NotNullable();

        // Restore the original relations.
        Create.ForeignKey("fk_subingredient_relation_ingredient_id")
            .FromTable("subingredient_relation")
            .ForeignColumn("ingredient_id")
            .ToTable("ingredients").PrimaryColumn("id");

        Create.ForeignKey("fk_subingredient_relation_subingredient_id")
            .FromTable("subingredient_relation")
            .ForeignColumn("subingredient_id")
            .ToTable("subingredients").PrimaryColumn("id");


        // Remove the composite primary key.
        Delete.PrimaryKey("pk_packed")
            .FromTable("packed");

        // Restore the original ID column.
        Alter.Table("packed")
            .AddColumn("id").AsInt32().Identity().NotNullable();

        // Restore the original primary key.
        Create.PrimaryKey("PK_packed_ingredients")
            .OnTable("packed")
            .Column("id");

        // Restore the original ordering column name.
        Rename.Column("order_no")
            .OnTable("packed")
            .To("order_number");

        // Restore the original table name.
        Rename.Table("packed").To("packed_ingredients");

        // Restore the previous image column name.
        Rename.Column("image_id")
            .OnTable("ingredients")
            .To("food_image_id");
        Rename.Column("image_id")
            .OnTable("meals")
            .To("food_image_id");

        // Recreate the users table.
        Create.Table("users")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("first_name").AsString(30).NotNullable()
            .WithColumn("last_name").AsString(100).NotNullable()
            .WithColumn("email").AsString(100).NotNullable()
            .WithColumn("password").AsString(100).NotNullable()
            .WithColumn("role").AsString(30).NotNullable()
            .WithColumn("pincode").AsString(100).Nullable()
            .WithColumn("role_approved").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("archived").AsBoolean().NotNullable().WithDefaultValue(false);

        // Recreate the classroom table.
        Create.Table("classroom")
            .WithColumn("class_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("class_name").AsString().NotNullable();

        // Recreate the children table.
        // A child is also a user and shares the same ID.
        Create.Table("children")
            .WithColumn("child_id").AsInt32().PrimaryKey()
            .WithColumn("first_name").AsString(100).NotNullable()
            .WithColumn("last_name").AsString(100).NotNullable()
            .WithColumn("class_id").AsInt32().Nullable();

        Create.ForeignKey("fk_children_user_id")
            .FromTable("children").ForeignColumn("child_id")
            .ToTable("users").PrimaryColumn("id");

        Create.ForeignKey("fk_children_class_id")
            .FromTable("children").ForeignColumn("class_id")
            .ToTable("classroom").PrimaryColumn("class_id");

        // Recreate the child_relation table.
        Create.Table("child_relation")
            .WithColumn("user_id").AsInt32().NotNullable()
            .WithColumn("child_id").AsInt32().NotNullable();

        Create.PrimaryKey("pk_child_relation")
            .OnTable("child_relation")
            .Columns("user_id", "child_id");

        Create.ForeignKey("fk_child_relation_user_id")
            .FromTable("child_relation").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");

        Create.ForeignKey("fk_child_relation_child_id")
            .FromTable("child_relation").ForeignColumn("child_id")
            .ToTable("children").PrimaryColumn("child_id");

        // Recreate the food_image table.
        Create.Table("food_image")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("image_id").AsString().Nullable()
            .WithColumn("user_id").AsInt32().NotNullable()
            .WithColumn("image_name").AsString().Nullable()
            .WithColumn("image_file_type").AsString().Nullable()
            .WithColumn("size").AsInt64().Nullable();

        Create.ForeignKey("fk_food_image_user_id")
            .FromTable("food_image").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");

        // Recreate chat_thread.
        Create.Table("chat_thread")
            .WithColumn("chat_thread_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("child_id").AsInt32().NotNullable();

        Create.ForeignKey("fk_chat_thread_child_id")
            .FromTable("chat_thread").ForeignColumn("child_id")
            .ToTable("children").PrimaryColumn("child_id");

        // Recreate message.
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

        // Recreate one_time_password.
        Create.Table("one_time_password")
            .WithColumn("code_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("generated_by").AsInt32().NotNullable()
            .WithColumn("code").AsString(6).NotNullable().Unique()
            .WithColumn("created_on").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("expires_on").AsDateTime().NotNullable()
            .WithColumn("used").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("used_by_user").AsInt32().Nullable()
            .WithColumn("child_user").AsInt32().Nullable();

        Create.ForeignKey("fk_otp_generated_by")
            .FromTable("one_time_password").ForeignColumn("generated_by")
            .ToTable("users").PrimaryColumn("id");

        Create.ForeignKey("fk_otp_used_by_user")
            .FromTable("one_time_password").ForeignColumn("used_by_user")
            .ToTable("users").PrimaryColumn("id");

        Create.ForeignKey("fk_otp_child_user")
            .FromTable("one_time_password").ForeignColumn("child_user")
            .ToTable("users").PrimaryColumn("id");

        // Restore foreign keys from meals.
        Create.ForeignKey("fk_meals_food_image_id")
            .FromTable("meals").ForeignColumn("food_image_id")
            .ToTable("food_image").PrimaryColumn("id");

        Create.ForeignKey("fk_meals_user_id")
            .FromTable("meals").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");

        // Restore foreign keys from ingredients.
        Create.ForeignKey("fk_ingredients_food_image_id")
            .FromTable("ingredients").ForeignColumn("food_image_id")
            .ToTable("food_image").PrimaryColumn("id");

        Create.ForeignKey("fk_ingredients_user_id")
            .FromTable("ingredients").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");

        // Restore foreign keys from subingredients.
        Create.ForeignKey("fk_subingredients_food_image_id")
            .FromTable("subingredients").ForeignColumn("food_image_id")
            .ToTable("food_image").PrimaryColumn("id");

        Create.ForeignKey("fk_subingredients_user_id")
            .FromTable("subingredients").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");
    }
}