using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CORE.APP.Domain;

namespace APP.Books.Domain;

public class Genre : Entity
{
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
    public string Name { get; set; }
    
    //For many-to-many relationship
    public List<BookGenre> BookGenres { get; private set; } = new List<BookGenre>();
    
    [NotMapped]
    public List<int> BookIds
    {
        get => BookGenres?.Select(b => b.BookId).ToList();
        set => BookGenres = value?.Select(id => new BookGenre { BookId = id }).ToList();
    }
}