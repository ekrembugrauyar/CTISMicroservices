using Microsoft.EntityFrameworkCore;

namespace APP.Books.Domain;

public class BooksDb : DbContext
{
    public BooksDb(DbContextOptions<BooksDb> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<BookGenre> BookGenres { get; set; }
    
    public DbSet<Author> Authors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BooksDb).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}