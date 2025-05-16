using System.Runtime.InteropServices.JavaScript;
using APP.Books.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Books.Features.Authors;

public class AuthorDeleteHandler : BooksDbHandler, IRequestHandler<AuthorDeleteRequest, CommandResponse>
{
    public AuthorDeleteHandler(BooksDb booksDb) : base(booksDb)
    {
    }

    public async Task<CommandResponse> Handle(AuthorDeleteRequest request, CancellationToken cancellationToken)
    {
        var entity = await _booksDb.Authors.Include(a => a.Books).SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        
        if (entity is null)
        {
            return Error($"Author with ID {request.Id} not found.");
        }

        if (entity.Books.Any())
        {
            return Error($"Author has books associated with them. Please remove the books before deleting the author.");
        }
        
        _booksDb.Authors.Remove(entity);
        await _booksDb.SaveChangesAsync(cancellationToken);
        
        return Success($"Author {entity.Name} {entity.Surname} deleted successfully.", entity.Id);
    }
}