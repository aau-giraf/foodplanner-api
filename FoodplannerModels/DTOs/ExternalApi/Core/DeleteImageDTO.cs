using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.DTOs.ExternalApi.Ai;

public class DeleteImageDTO
{
	[Required]
	public int ImageId { get; set; }
}
