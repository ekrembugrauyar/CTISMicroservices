using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CORE.APP.Features;
using MediatR;

namespace APP.Books.Features.Authors;

public class AuthorUpdateRequest : Request, IRequest<CommandResponse>
{
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
    public string Surname { get; set; }
}