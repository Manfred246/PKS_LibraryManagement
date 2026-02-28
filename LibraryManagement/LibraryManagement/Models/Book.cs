using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public int GenreId { get; set; }

        [Required]
        public int PublishYear { get; set; }

        [MaxLength(20)]
        public string? ISBN { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }
        
        public virtual Author? Author { get; set; }
        public virtual Genre? Genre { get; set; }
    }
}