using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account;

// Child DTO with error messages 
public class ChildrenDTO
{
    public int ChildId { get; set; }

    [Required(ErrorMessage = "Fornavn er påkrævet")]
    [StringLength(100, ErrorMessage = "Fornavn er for langt")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Efternavn er påkrævet")]
    [StringLength(100, ErrorMessage = "Efternavn er for langt")]
    public required string LastName { get; set; }

    public int ParentId { get; set; }

    public int? ClassId { get; set; }
}