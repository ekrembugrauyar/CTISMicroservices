using APP.Books.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Books.Features.Books;

public class BookDeleteHandler : BooksDbHandler, IRequestHandler<BookDeleteRequest, CommandResponse>
{
    public BookDeleteHandler(BooksDb booksDb) : base(booksDb)
    {
    }

    public async Task<CommandResponse> Handle(BookDeleteRequest request, CancellationToken cancellationToken)
    {
        var entity = await _booksDb.Books.Include(b => b.BookGenres).ThenInclude(bg => bg.Genre).SingleOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return Error($"Book with id {request.Id} not found.");
        }
        
        _booksDb.RemoveRange(entity.BookGenres);
        _booksDb.Books.Remove(entity);
        
        await _booksDb.SaveChangesAsync(cancellationToken);
        return Success($"Book with id {request.Id} deleted", entity.Id);
    }
}