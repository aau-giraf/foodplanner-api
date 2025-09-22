using FoodplannerModels.Lunchbox;

namespace Test.Builder;

public class MealBuilder : Meal
{
   public MealBuilder()
   {
      this.Id = 99999;
      this.Name = "testMeal";
      this.Date = "11/11/50";
   }

   public MealBuilder WithId(int id)
   {
      this.Id = id;
      return this;
   }

   public MealBuilder WithName(string name)
   {
      this.Name = name;
      return this;
   }

   public MealBuilder WithUserId(int userId)
   {
      this.User_id = userId;
      return this;
   }

   public MealBuilder WithFoodImageId(int? foodImageId)
   {
      this.Food_image_id = foodImageId;
      return this;
   }

   public MealBuilder WithDate(string date)
   {
      this.Date = date;
      return this;
   }

   public Meal Build()
   {
      return new Meal
      {
         Id = this.Id,
         Name = this.Name,
         User_id = this.User_id,
         Food_image_id = this.Food_image_id,
         Date = this.Date,
      };
   }
}
