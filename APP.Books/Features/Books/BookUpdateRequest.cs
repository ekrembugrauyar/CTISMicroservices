using System.ComponentModel.DataAnnotations;
using CORE.APP.Features;
using MediatR;

namespace APP.Books.Features.Books;

public class BookUpdateRequest : Request, IRequest<CommandResponse>
{
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255)]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    public DateOnly PublishDate { get; set; }
    
    public short? NumberOfPages { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    public bool IsTopSeller { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    public List<int> GenreIds { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    public int AuthorId { get; set; }
}