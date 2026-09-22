using FluentMigrator;

namespace FoodplannerDataAccessSql.Migrations
{
    //fourth iteration of the database
    [Migration(4)]
    public class AddedColmnToTable : Migration
    {
        //changes when migration is run
        public override void Up()
        {
            //alter table "packed_ingredients" by adding another column (attribute) to the table
            Alter.Table("packed_ingredients")
                .AddColumn("order_number").AsInt32().NotNullable().WithDefaultValue(0); //has a defult value of 0
        }

        //removal of changes when migrations are rolled back
        public override void Down()
        {
            //removing the column added
            Delete.Column("order_number").FromTable("packed_ingredients");
        }
    }
}