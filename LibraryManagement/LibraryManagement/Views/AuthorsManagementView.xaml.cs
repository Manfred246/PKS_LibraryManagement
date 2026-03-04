using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace LibraryManagement.Views
{
    public partial class AuthorsManagementView : Window
    {
        private readonly LibraryContext _context;

        public AuthorsManagementView(LibraryContext context)
        {
            InitializeComponent();
            _context = context;
            _context.Authors.Load();
            AuthorsGrid.ItemsSource = _context.Authors.Local.ToObservableCollection();
        }

        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            var author = new Author();
            _context.Authors.Add(author);
            AuthorsGrid.ScrollIntoView(author);
            AuthorsGrid.BeginEdit();
        }

        private void DeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            var author = AuthorsGrid.SelectedItem as Author;
            if (author != null)
            {                
                if (_context.BookAuthors.Any(ba => ba.AuthorId == author.Id))
                {
                    MessageBox.Show("Нельзя удалить автора, у которого есть книги", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Удалить автора {author.FullName}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                    _context.Authors.Remove(author);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                var authorsToCheck = _context.Authors.Local
                    .Where(a => !string.IsNullOrWhiteSpace(a.FirstName) &&
                               !string.IsNullOrWhiteSpace(a.LastName))
                    .ToList();

                foreach (var author in authorsToCheck)
                {                    
                    bool isDuplicate = _context.Authors
                        .Any(a => a.Id != author.Id &&
                                 a.FirstName.ToLower() == author.FirstName.ToLower() &&
                                 a.LastName.ToLower() == author.LastName.ToLower() &&
                                 a.BirthDate == author.BirthDate);

                    if (isDuplicate)
                    {
                        MessageBox.Show($"Автор '{author.FullName}' с датой рождения {author.BirthDate:dd.MM.yyyy} уже существует в базе данных.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

                        AuthorsGrid.SelectedItem = author;
                        AuthorsGrid.ScrollIntoView(author);
                        return;
                    }
                }

                _context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}