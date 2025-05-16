using System.Globalization;
using APP.Books.Domain;
using CORE.APP.Features;

namespace APP.Books.Features;

public abstract class BooksDbHandler : Handler
{
    protected readonly BooksDb _booksDb;
    
    protected BooksDbHandler(BooksDb booksDb) : base(new CultureInfo("en-US"))
    {
        _booksDb = booksDb;
    }
}