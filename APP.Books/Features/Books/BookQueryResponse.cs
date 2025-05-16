using APP.Books.Features.Authors;
using CORE.APP.Features;

namespace APP.Books.Features.Books;

public class BookQueryResponse : QueryResponse
{
    public string Name { get; set; }
    
    public DateOnly PublishDate { get; set; }
    
    public short? NumberOfPages { get; set; }
    
    public decimal Price { get; set; }
    
    public bool IsTopSeller { get; set; }
    
    public string GenreNames { get; set; }
    
    public AuthorQueryResponse Author { get; set; }
}