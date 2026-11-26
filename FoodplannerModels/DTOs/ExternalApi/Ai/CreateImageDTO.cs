using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.DTOs.ExternalApi.Ai;

public class CreateImageDTO
{
	[Required(ErrorMessage = "Prompt er påkrævet")]
	public string Prompt { get; set; }
	[Required]
	public int MapId { get; set; }
	[Required]
	public string Type { get; set; }
	[Required]
	public string ImgId { get; set; }
	[Required]
	public string OriginApp { get; set; }
}
