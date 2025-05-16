using APP.Books.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Books.Features.Books;

public class BookCreateHandler : BooksDbHandler, IRequestHandler<BookCreateRequest, CommandResponse>
{
    public BookCreateHandler(BooksDb booksDb) : base(booksDb)
    {
    }

    public async Task<CommandResponse> Handle(BookCreateRequest request, CancellationToken cancellationToken)
    {
        if (await _booksDb.Books.AnyAsync(b => b.Name.ToLower() == request.Name.ToLower().Trim(), cancellationToken: cancellationToken))
        {
            return Error($"Book {request.Name} already exists");
        }
        
        var item = new Book()
        {
            Name = request.Name.Trim(),
            PublishDate = request.PublishDate,
            NumberOfPages = request.NumberOfPages,
            Price = request.Price,
            IsTopSeller = request.IsTopSeller,
            GenreIds = request.GenreIds,
            AuthorId = request.AuthorId
        };
        
        _booksDb.Books.Add(item);
        await _booksDb.SaveChangesAsync(cancellationToken);
        return Success($"Book {request.Name} created", item.Id);
    }
}