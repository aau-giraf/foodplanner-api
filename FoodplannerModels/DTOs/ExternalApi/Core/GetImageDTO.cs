using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.DTOs.ExternalApi.Ai;

public class GetImageDTO
{
	[Required]
	public string ImageId { get; set; }
}
