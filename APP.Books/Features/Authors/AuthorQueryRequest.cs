using APP.Books.Domain;
using CORE.APP.Features;
using MediatR;

namespace APP.Books.Features.Authors;

public class AuthorQueryRequest : Request, IRequest<IQueryable<AuthorQueryResponse>>
{
    
}