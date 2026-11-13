using System.Text.Json.Serialization;

namespace FoodplannerModels.Account
{
   // Class for errors in controllers
   public class ErrorResponse
   {
        [JsonPropertyName("Message")]
        public String[]? Message { get; set; }

        [JsonPropertyName("Email")]
        public String[]? Email { get; set; }
    }
}