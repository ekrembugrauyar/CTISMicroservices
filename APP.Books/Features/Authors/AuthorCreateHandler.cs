using APP.Books.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Books.Features.Authors;

public class AuthorCreateHandler : BooksDbHandler, IRequestHandler<AuthorCreateRequest, CommandResponse>
{
    public AuthorCreateHandler(BooksDb booksDb) : base(booksDb)
    {
    }

    public async Task<CommandResponse> Handle(AuthorCreateRequest request, CancellationToken cancellationToken)
    {
        if (await _booksDb.Authors.AnyAsync(a => a.Name.ToLower() == request.Name.ToLower().Trim() && a.Surname.ToLower() == request.Surname.ToLower().Trim(), cancellationToken: cancellationToken))
        {
            return Error($"Author {request.Name} {request.Surname} already exists.");
        }
        
        var entity = new Author
        {
            Name = request.Name.Trim(),
            Surname = request.Surname.Trim()
        };
        
        _booksDb.Authors.Add(entity);
        await _booksDb.SaveChangesAsync(cancellationToken);
        return Success($"Author {request.Name} {request.Surname} created successfully.", entity.Id);
        
    }
}