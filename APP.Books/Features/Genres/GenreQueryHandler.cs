using APP.Books.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Books.Features.Genres;

public class GenreQueryHandler : BooksDbHandler, IRequestHandler<GenreQueryRequest, IQueryable<GenreQueryResponse>>
{
    public GenreQueryHandler(BooksDb booksDb) : base(booksDb)
    {
    }

    public Task<IQueryable<GenreQueryResponse>> Handle(GenreQueryRequest request, CancellationToken cancellationToken)
    {
        var query = _booksDb.Genres.Include(g => g.BookGenres).ThenInclude(bg => bg.Book).OrderBy(g => g.Name).Select(g => new GenreQueryResponse()
        {
            Id = g.Id,
            Name = g.Name,
            
            BookNames = string.Join(", ", g.BookGenres.Select(bg => bg.Book.Name)),
            BookCount = g.BookGenres.Count(),
        });
        return Task.FromResult(query);
    }
}