using APP.Books.Domain;
using APP.Books.Features.Books;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Books.Features.Authors;

public class AuthorQueryHandler : BooksDbHandler, IRequestHandler<AuthorQueryRequest, IQueryable<AuthorQueryResponse>>
{
    public AuthorQueryHandler(BooksDb booksDb) : base(booksDb)
    {
    }

    public Task<IQueryable<AuthorQueryResponse>> Handle(AuthorQueryRequest request, CancellationToken cancellationToken)
    {
        var query = _booksDb.Authors.Include(a => a.Books).ThenInclude(b => b.BookGenres).ThenInclude(bg => bg.Genre).OrderBy(a => a.Name).ThenBy(a => a.Surname).Select(a => new AuthorQueryResponse()
        {
            Id = a.Id,
            Name = a.Name,
            Surname = a.Surname,
            BookCount = a.Books.Count(),
        });
        
        return Task.FromResult(query);
    }
}