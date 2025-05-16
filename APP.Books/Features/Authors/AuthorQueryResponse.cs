using APP.Books.Features.Books;
using CORE.APP.Features;

namespace APP.Books.Features.Authors;

public class AuthorQueryResponse : QueryResponse
{
    public string Name { get; set; }

    public string Surname { get; set; }
    
    public int BookCount { get; set; }
    
}