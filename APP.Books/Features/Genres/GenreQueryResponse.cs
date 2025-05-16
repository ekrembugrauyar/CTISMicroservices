using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using APP.Books.Domain;
using CORE.APP.Features;

namespace APP.Books.Features.Genres;

public class GenreQueryResponse : QueryResponse
{
    public string Name { get; set; }
    
    public string BookNames { get; set; }
    
    public int BookCount { get; set; }
}