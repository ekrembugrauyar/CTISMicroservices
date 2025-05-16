using CORE.APP.Features;
using MediatR;

namespace APP.Books.Features.Books;

public class BookDeleteRequest : Request, IRequest<CommandResponse>
{
    
}