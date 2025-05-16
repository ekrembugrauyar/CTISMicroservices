using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using APP.Books.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Books.Features.Genres;

public class GenreUpdateHandler : BooksDbHandler, IRequestHandler<GenreUpdateRequest, CommandResponse>
{
    public GenreUpdateHandler(BooksDb booksDb) : base(booksDb)
    {
    }

    public async Task<CommandResponse> Handle(GenreUpdateRequest request, CancellationToken cancellationToken)
    {
        if (await _booksDb.Genres.AnyAsync(g => g.Id != request.Id && g.Name.ToLower() == request.Name.Trim().ToLower(), cancellationToken))
        {
            return Error($"Genre {request.Name} already exists");
        }
        
        var entity = await _booksDb.Genres.Include(g => g.BookGenres).ThenInclude(bg => bg.Book).SingleOrDefaultAsync(g => g.Id == request.Id, cancellationToken);
        
        if (entity is null)
        {
            return Error($"Genre with id {request.Id} does not exist");
        }
        
        _booksDb.BookGenres.RemoveRange(entity.BookGenres);
        
        entity.Name = request.Name.Trim();
        entity.BookIds = request.BookIds;

        _booksDb.Genres.Update(entity);
        await _booksDb.SaveChangesAsync(cancellationToken);
        
        return Success($"Genre with name {entity.Name} updated", entity.Id);
    }
}