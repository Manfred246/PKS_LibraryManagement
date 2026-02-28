using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Genre> Genres => Set<Genre>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=LibraryDB;Trusted_Connection=True;");
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

                entity.HasOne(e => e.Author)
                    .WithMany(e => e.Books)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
                
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
                new Book { Id = 1, Title = "Преступление и наказание", AuthorId = 1, GenreId = 1, PublishYear = 1866, ISBN = "978-5-17-123456-7", QuantityInStock = 5 },
                new Book { Id = 2, Title = "Приключения Шерлока Холмса", AuthorId = 2, GenreId = 2, PublishYear = 1892, ISBN = "978-5-04-123456-8", QuantityInStock = 3 }
            );
        }
    }
}