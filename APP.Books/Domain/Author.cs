using System.ComponentModel.DataAnnotations;
using CORE.APP.Domain;

namespace APP.Books.Domain;

public class Author : Entity
{
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
    public string Surname { get; set; }
    
    public List<Book> Books { get; set; } = new List<Book>();
}