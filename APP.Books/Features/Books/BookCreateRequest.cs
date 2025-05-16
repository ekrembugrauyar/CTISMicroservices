using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CORE.APP.Features;
using MediatR;

namespace APP.Books.Features.Books;

public class BookCreateRequest : Request, IRequest<CommandResponse>
{
    [JsonIgnore]
    public override int Id { get => base.Id; set => base.Id = value; }
    
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
    
    [Required(ErrorMessage = "{0} is required")]
    public List<int> GenreIds { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    public int AuthorId { get; set; }
}