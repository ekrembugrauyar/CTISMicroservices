using CORE.APP.Features;
using MediatR;

namespace APP.Books.Features.Authors;

public class AuthorDeleteRequest : Request, IRequest<CommandResponse>
{
    
}