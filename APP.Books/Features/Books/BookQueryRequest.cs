using CORE.APP.Features;
using MediatR;

namespace APP.Books.Features.Books;

public class BookQueryRequest : Request, IRequest<IQueryable<BookQueryResponse>>
{
    
}