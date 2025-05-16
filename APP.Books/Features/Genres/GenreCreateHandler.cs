using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using APP.Books.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Books.Features.Genres;

public class GenreCreateHandler : BooksDbHandler, IRequestHandler<GenreCreateRequest, CommandResponse>
{
    public GenreCreateHandler(BooksDb booksDb) : base(booksDb)
    {
    }

    public async Task<CommandResponse> Handle(GenreCreateRequest request, CancellationToken cancellationToken)
    {
        if (await _booksDb.Genres.AnyAsync(g => g.Name.ToLower() == request.Name.Trim().ToLower(), cancellationToken))
        {
            return Error($"Genre {request.Name} already exists");
        }
        
        var entity = new Genre
        {
            Name = request.Name.Trim(),
            BookIds = request.BookIds,
        };

        await _booksDb.Genres.AddAsync(entity, cancellationToken);
        await _booksDb.SaveChangesAsync(cancellationToken);
        
        return Success($"Genre with id {entity.Id} created", entity.Id);
    }
}