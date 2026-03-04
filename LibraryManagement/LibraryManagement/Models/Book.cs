using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LibraryManagement.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int GenreId { get; set; }

        [Required]
        public int PublishYear { get; set; }

        [MaxLength(20)]
        public string? ISBN { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }

        public virtual Genre? Genre { get; set; }

        public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new HashSet<BookAuthor>();
        
        public ICollection<Author> Authors
        {
            get
            {
                return BookAuthors?.Select(ba => ba.Author).Where(a => a != null).ToList() ?? new List<Author>();
            }
        }
        
        public string AuthorsDisplay
        {
            get
            {
                if (Authors != null && Authors.Any())
                {
                    return string.Join(", ", Authors.Select(a => a.FullName));
                }
                return "Нет авторов";
            }
        }
    }
}