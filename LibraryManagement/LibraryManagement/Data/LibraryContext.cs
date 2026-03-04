using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=LibraryDB_MultiAuthor;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ISBN)
                    .HasMaxLength(20);

                entity.Property(e => e.PublishYear)
                    .IsRequired();

                entity.Property(e => e.QuantityInStock)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.HasOne(e => e.Genre)
                    .WithMany(e => e.Books)
                    .HasForeignKey(e => e.GenreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Country)
                    .HasMaxLength(100);
            });
            
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);
            });
            
            modelBuilder.Entity<BookAuthor>(entity =>
            {
                entity.HasKey(ba => new { ba.BookId, ba.AuthorId });

                entity.HasOne(ba => ba.Book)
                    .WithMany(b => b.BookAuthors)
                    .HasForeignKey(ba => ba.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ba => ba.Author)
                    .WithMany(a => a.BookAuthors)
                    .HasForeignKey(ba => ba.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Роман", Description = "Художественное произведение большого объема" },
                new Genre { Id = 2, Name = "Детектив", Description = "Жанр, посвященный расследованию преступлений" },
                new Genre { Id = 3, Name = "Фантастика", Description = "Жанр о вымышленных мирах и технологиях" }
            );

            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FirstName = "Федор", LastName = "Достоевский", BirthDate = new DateTime(1821, 11, 11), Country = "Россия" },
                new Author { Id = 2, FirstName = "Артур", LastName = "Конан Дойл", BirthDate = new DateTime(1859, 5, 22), Country = "Великобритания" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Преступление и наказание", GenreId = 1, PublishYear = 1866, ISBN = "978-5-17-123456-7", QuantityInStock = 5 },
                new Book { Id = 2, Title = "Приключения Шерлока Холмса", GenreId = 2, PublishYear = 1892, ISBN = "978-5-04-123456-8", QuantityInStock = 3 }
            );

            modelBuilder.Entity<BookAuthor>().HasData(
                new BookAuthor { BookId = 1, AuthorId = 1 },
                new BookAuthor { BookId = 2, AuthorId = 2 } 
            );
        }
    }
}