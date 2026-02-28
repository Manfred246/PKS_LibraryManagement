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
        public ObservableCollection<Author> Authors { get; set; }
        public ObservableCollection<Genre> Genres { get; set; }

        public BookDialogView(LibraryContext context, Book book)
        {
            InitializeComponent();
            _context = context;
            Book = book;

            _context.Authors.Load();
            _context.Genres.Load();

            Authors = new ObservableCollection<Author>(_context.Authors.Local.ToObservableCollection());
            Genres = new ObservableCollection<Genre>(_context.Genres.Local.ToObservableCollection());

            if (Book.Id == 0)
            {
                Book.Author = Authors.FirstOrDefault();
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

            if (Book.Author == null)
            {
                MessageBox.Show("Выберите автора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Book.Genre == null)
            {
                MessageBox.Show("Выберите жанр", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Book.AuthorId = Book.Author.Id;
            Book.GenreId = Book.Genre.Id;

            if (Book.QuantityInStock < 0)
            {
                MessageBox.Show("Количество не может быть отрицательным", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Book.Id == 0)
                _context.Books.Add(Book);

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
}