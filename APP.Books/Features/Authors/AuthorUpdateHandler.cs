using APP.Books.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Books.Features.Authors;

public class AuthorUpdateHandler : BooksDbHandler, IRequestHandler<AuthorUpdateRequest, CommandResponse>
{
    public AuthorUpdateHandler(BooksDb booksDb) : base(booksDb)
    {
    }

    public async Task<CommandResponse> Handle(AuthorUpdateRequest request, CancellationToken cancellationToken)
    {
        if (await _booksDb.Authors.AnyAsync(a => a.Id != request.Id && a.Name.ToLower() == request.Name.ToLower().Trim() && a.Surname.ToLower() == request.Surname.ToLower().Trim(), cancellationToken: cancellationToken))
        {
            return Error($"Author {request.Name} {request.Surname} with Id {request.Id} already exists.");
        }
        
        var entity = await _booksDb.Authors.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken: cancellationToken);
        
        if (entity == null)
        {
            return Error($"Author with Id {request.Id} not found.");
        }
        
        entity.Name = request.Name.Trim();
        entity.Surname = request.Surname.Trim();

        _booksDb.Authors.Update(entity);
        await _booksDb.SaveChangesAsync(cancellationToken);
        
        return Success($"Author {request.Name} {request.Surname} updated successfully.", entity.Id);
        
    }
}