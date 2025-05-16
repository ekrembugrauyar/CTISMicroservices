using APP.Books.Domain;
using APP.Books.Features.Authors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Books.Features.Books;

public class BookQueryHandler : BooksDbHandler, IRequestHandler<BookQueryRequest, IQueryable<BookQueryResponse>>
{
    public BookQueryHandler(BooksDb booksDb) : base(booksDb)
    {
    }

    public Task<IQueryable<BookQueryResponse>> Handle(BookQueryRequest request, CancellationToken cancellationToken)
    {
        IQueryable<BookQueryResponse> query = _booksDb.Books.Include(b => b.Author).Include(b => b.BookGenres).ThenInclude(bg => bg.Genre).OrderBy(b => b.Name).Select(b => new BookQueryResponse()
        {
            Id = b.Id,
            Name = b.Name,
            PublishDate = b.PublishDate,
            NumberOfPages = b.NumberOfPages,
            Price = b.Price,
            IsTopSeller = b.IsTopSeller,
            
            GenreNames = string.Join(", ", b.BookGenres.Select(bg => bg.Genre.Name)),
            
            Author = new AuthorQueryResponse()
            {
                Name = b.Author.Name,
                Surname = b.Author.Surname,
                BookCount = b.Author.Books.Count(),
                Id = b.Author.Id
            }
        });
        return Task.FromResult(query);
    }
}