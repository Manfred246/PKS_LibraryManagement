using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LibraryManagement.Views
{
    public partial class BookDialogView : Window
    {
        private readonly LibraryContext _context;

        public Book Book { get; }
        public ObservableCollection<AuthorViewModel> AvailableAuthors { get; set; }
        public ObservableCollection<Genre> Genres { get; set; }

        public BookDialogView(LibraryContext context, Book book)
        {
            InitializeComponent();
            _context = context;
            Book = book;
            
            _context.Genres.Load();
            _context.Authors.Load();

            Genres = new ObservableCollection<Genre>(_context.Genres.Local.ToObservableCollection());
            
            var allAuthors = _context.Authors.Local.ToObservableCollection();
            AvailableAuthors = new ObservableCollection<AuthorViewModel>();
            
            var selectedAuthorIds = new HashSet<int>();
            if (Book.Id != 0 && Book.BookAuthors != null)
            {
                selectedAuthorIds = Book.BookAuthors.Select(ba => ba.AuthorId).ToHashSet();
            }

            foreach (var author in allAuthors)
            {
                AvailableAuthors.Add(new AuthorViewModel
                {
                    Id = author.Id,
                    FullName = author.FullName,
                    IsSelected = selectedAuthorIds.Contains(author.Id)
                });
            }

            if (Book.Id == 0)
            {
                Book.Genre = Genres.FirstOrDefault();
            }

            DataContext = this;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {            
            if (Book.PublishYear < 1000 || Book.PublishYear > DateTime.Now.Year + 1)
            {
                MessageBox.Show($"Укажите корректный год издания (от 1000 до {DateTime.Now.Year + 1})",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Book.Title))
            {
                MessageBox.Show("Введите название книги", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedAuthors = AvailableAuthors.Where(a => a.IsSelected).ToList();
            if (!selectedAuthors.Any())
            {
                MessageBox.Show("Выберите хотя бы одного автора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Book.Genre == null)
            {
                MessageBox.Show("Выберите жанр", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Book.QuantityInStock < 0)
            {
                MessageBox.Show("Количество не может быть отрицательным", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Book.GenreId = Book.Genre.Id;
            
            if (Book.Id == 0)
            {                
                _context.Books.Add(Book);

                foreach (var authorVm in selectedAuthors)
                {
                    _context.BookAuthors.Add(new BookAuthor
                    {
                        Book = Book,
                        AuthorId = authorVm.Id
                    });
                }
            }
            else
            {                
                var existingLinks = _context.BookAuthors.Where(ba => ba.BookId == Book.Id);
                _context.BookAuthors.RemoveRange(existingLinks);

                foreach (var authorVm in selectedAuthors)
                {
                    _context.BookAuthors.Add(new BookAuthor
                    {
                        BookId = Book.Id,
                        AuthorId = authorVm.Id
                    });
                }
            }

            DialogResult = true;
            Close();
        }
        
        private void PublishYear_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!e.Text.All(char.IsDigit))
            {
                e.Handled = true;
            }
        }

        private void PublishYear_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Tab ||
                e.Key == Key.Enter || e.Key == Key.Left || e.Key == Key.Right ||
                e.Key == Key.Home || e.Key == Key.End)
            {
                return;
            }

            if ((e.Key < Key.D0 || e.Key > Key.D9) && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9))
            {
                e.Handled = true;
            }
        }

        private void PublishYear_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));

                if (!text.All(char.IsDigit))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void Quantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!e.Text.All(char.IsDigit))
            {
                e.Handled = true;
            }
        }

        private void Quantity_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Tab ||
                e.Key == Key.Enter || e.Key == Key.Left || e.Key == Key.Right ||
                e.Key == Key.Home || e.Key == Key.End)
            {
                return;
            }

            if ((e.Key < Key.D0 || e.Key > Key.D9) && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9))
            {
                e.Handled = true;
            }
        }

        private void Quantity_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));

                if (!text.All(char.IsDigit))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class AuthorViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}