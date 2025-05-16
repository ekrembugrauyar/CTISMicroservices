using CORE.APP.Features;
using MediatR;

namespace APP.Books.Features.Genres;

public class GenreDeleteRequest : Request, IRequest<CommandResponse>
{
    
}