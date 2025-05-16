using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using APP.Books.Domain;
using CORE.APP.Features;
using MediatR;

namespace APP.Books.Features.Genres;

public class GenreCreateRequest : Request, IRequest<CommandResponse>
{
    [JsonIgnore]
    public override int Id { get => base.Id; set => base.Id = value; }
    
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
    public string Name { get; set; }

    public List<int> BookIds { get; set; }
}