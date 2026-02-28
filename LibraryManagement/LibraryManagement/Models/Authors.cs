using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new HashSet<Book>();

        public string FullName => $"{FirstName} {LastName}";
    }
}