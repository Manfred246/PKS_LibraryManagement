using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace LibraryManagement.Views
{
    public partial class GenresManagementView : Window
    {
        private readonly LibraryContext _context;

        public GenresManagementView(LibraryContext context)
        {
            InitializeComponent();
            _context = context;
            _context.Genres.Load();
            GenresGrid.ItemsSource = _context.Genres.Local.ToObservableCollection();
        }

        private void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            var genre = new Genre();
            _context.Genres.Add(genre);
            GenresGrid.ScrollIntoView(genre);
            GenresGrid.BeginEdit();
        }

        private void DeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            var genre = GenresGrid.SelectedItem as Genre;
            if (genre != null)
            {
                if (_context.Books.Any(b => b.GenreId == genre.Id))
                {
                    MessageBox.Show("Нельзя удалить жанр, который используется книгами", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Удалить жанр {genre.Name}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                    _context.Genres.Remove(genre);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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