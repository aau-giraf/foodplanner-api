namespace FoodplannerModels.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Children {
    [Key]
    public int ChildId { get; set; }

    [Required(ErrorMessage = "Fornavn er påkrævet")]
    [StringLength(100, ErrorMessage = "Fornavn er for langt")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Efternavn er påkrævet")]
    [StringLength(100, ErrorMessage = "Efternavn er for langt")]
    public required string LastName { get; set; }

    [ForeignKey("User")]
    public int parentId { get; set; }
    public User user { get; set; }

    [ForeignKey("Classroom")]
    public int classId { get; set; }
    public Classroom Classroom { get; set; }

}

