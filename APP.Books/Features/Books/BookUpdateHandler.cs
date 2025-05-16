using APP.Books.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Books.Features.Books;

public class BookUpdateHandler : BooksDbHandler, IRequestHandler<BookUpdateRequest, CommandResponse>
{
    public BookUpdateHandler(BooksDb booksDb) : base(booksDb)
    {
    }

    public async Task<CommandResponse> Handle(BookUpdateRequest request, CancellationToken cancellationToken)
    {
        if (await _booksDb.Books.AnyAsync(b => b.Id != request.Id && b.Name.ToLower() == request.Name.ToLower().Trim(), cancellationToken: cancellationToken))
        {
            return Error($"Book {request.Name} already exists");
        }
        
        var entity = await _booksDb.Books.Include(b => b.BookGenres).ThenInclude(bg => bg.Genre).SingleOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return Error($"Book with id {request.Id} does not exist");
        }
        
        _booksDb.BookGenres.RemoveRange(entity.BookGenres);
        
        entity.Name = request.Name.Trim();
        entity.NumberOfPages = request.NumberOfPages;
        entity.PublishDate = request.PublishDate;
        entity.IsTopSeller = request.IsTopSeller;
        entity.Price = request.Price;
        entity.GenreIds = request.GenreIds;
        entity.AuthorId = request.AuthorId;
        
        _booksDb.Books.Update(entity);
        
        await _booksDb.SaveChangesAsync(cancellationToken);
        return Success($"Book with name {entity.Name} updated", entity.Id);
    }
    
}