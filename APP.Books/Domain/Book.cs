using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using CORE.APP.Domain;

namespace APP.Books.Domain;

public class Book : Entity
{
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255)]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    [DisplayName("Publish Date")]
    public DateOnly PublishDate { get; set; }
    
    public short? NumberOfPages { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    [DisplayName("Is Top Seller")]
    public bool IsTopSeller { get; set; }
    
    //For many-to-many relationship
    public List<BookGenre> BookGenres { get; private set; } = new List<BookGenre>();

    [NotMapped]
    public List<int> GenreIds
    {
        get => BookGenres?.Select(bg => bg.GenreId).ToList();
        set => BookGenres = value?.Select(id => new BookGenre { GenreId = id }).ToList();
    }
    
    [Required(ErrorMessage = "{0} is required")]
    [DisplayName("Author Id")]
    public int AuthorId { get; set; }
    
    public Author Author { get; set; }
}