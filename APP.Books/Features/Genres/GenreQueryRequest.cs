using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using APP.Books.Domain;
using CORE.APP.Features;
using MediatR;

namespace APP.Books.Features.Genres;

public class GenreQueryRequest : Request, IRequest<IQueryable<GenreQueryResponse>>
{
    
}