using APP.Books.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Books.Features.Genres;

public class GenreDeleteHandler : BooksDbHandler, IRequestHandler<GenreDeleteRequest, CommandResponse>
{
    public GenreDeleteHandler(BooksDb booksDb) : base(booksDb)
    {
    }

    public async Task<CommandResponse> Handle(GenreDeleteRequest request, CancellationToken cancellationToken)
    {
        var entity = await _booksDb.Genres.Include(g => g.BookGenres).ThenInclude(bg => bg.Book).SingleOrDefaultAsync(g => g.Id == request.Id, cancellationToken: cancellationToken);
        
        if (entity is null)
        {
            return Error($"Genre with id {request.Id} not found.");
        }
        
        if (entity.BookGenres.Any())
        {
            return Error($"Genre with id {request.Id} cannot be deleted because it is used by {string.Join(", ", entity.BookGenres.Select(bg => bg.Book.Name))}");
        }
        
        _booksDb.BookGenres.RemoveRange(entity.BookGenres);
        
        _booksDb.Genres.Remove(entity);
        await _booksDb.SaveChangesAsync(cancellationToken);
        
        return Success($"Genre with id {request.Id} deleted", entity.Id);
    }
}