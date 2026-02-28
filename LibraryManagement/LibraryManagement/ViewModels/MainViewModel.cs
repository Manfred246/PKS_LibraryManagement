using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System;

namespace LibraryManagement.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly LibraryContext _context;
        private ObservableCollection<Book>? _books;
        private ObservableCollection<Author>? _authors;
        private ObservableCollection<Genre>? _genres;
        private Book? _selectedBook;
        private Author? _selectedAuthorFilter;
        private Genre? _selectedGenreFilter;
        private string _searchText = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel()
        {
            _context = new LibraryContext();
            _context.Database.EnsureCreated();

            LoadData();

            AddBookCommand = new RelayCommand(AddBook);
            EditBookCommand = new RelayCommand(EditBook, CanEditOrDeleteBook);
            DeleteBookCommand = new RelayCommand(DeleteBook, CanEditOrDeleteBook);
            ManageAuthorsCommand = new RelayCommand(ManageAuthors);
            ManageGenresCommand = new RelayCommand(ManageGenres);
        }

        private void LoadData()
        {
            _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Load();

            _context.Authors.Load();
            _context.Genres.Load();

            Books = _context.Books.Local.ToObservableCollection();
            Authors = _context.Authors.Local.ToObservableCollection();
            Genres = _context.Genres.Local.ToObservableCollection();
        }

        public ObservableCollection<Book>? Books
        {
            get => ApplyFilters();
            set
            {
                _books = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Author>? Authors
        {
            get => _authors;
            set
            {
                _authors = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Genre>? Genres
        {
            get => _genres;
            set
            {
                _genres = value;
                OnPropertyChanged();
            }
        }

        public Book? SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public Author? SelectedAuthorFilter
        {
            get => _selectedAuthorFilter;
            set
            {
                _selectedAuthorFilter = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Books));
            }
        }

        public Genre? SelectedGenreFilter
        {
            get => _selectedGenreFilter;
            set
            {
                _selectedGenreFilter = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Books));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Books));
            }
        }

        private ObservableCollection<Book>? ApplyFilters()
        {
            if (_books == null) return null;

            var filtered = _books.AsEnumerable();

            if (SelectedAuthorFilter != null)
                filtered = filtered.Where(b => b.AuthorId == SelectedAuthorFilter.Id);

            if (SelectedGenreFilter != null)
                filtered = filtered.Where(b => b.GenreId == SelectedGenreFilter.Id);

            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(b =>
                    b.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            return new ObservableCollection<Book>(filtered);
        }

        public ICommand AddBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand ManageAuthorsCommand { get; }
        public ICommand ManageGenresCommand { get; }

        private void AddBook(object? parameter)
        {
            var dialog = new Views.BookDialogView(_context, new Book());
            if (dialog.ShowDialog() == true)
            {
                _context.SaveChanges();
                OnPropertyChanged(nameof(Books));
            }
        }

        private void EditBook(object? parameter)
        {
            if (SelectedBook == null) return;

            var dialog = new Views.BookDialogView(_context, SelectedBook);
            if (dialog.ShowDialog() == true)
            {
                _context.SaveChanges();
                OnPropertyChanged(nameof(Books));
            }
        }

        private bool CanEditOrDeleteBook(object? parameter)
        {
            return SelectedBook != null;
        }

        private void DeleteBook(object? parameter)
        {
            if (SelectedBook == null) return;

            var result = System.Windows.MessageBox.Show(
                $"Удалить книгу '{SelectedBook.Title}'?",
                "Подтверждение удаления",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _context.Books.Remove(SelectedBook);
                _context.SaveChanges();
                OnPropertyChanged(nameof(Books));
            }
        }

        private void ManageAuthors(object? parameter)
        {
            var dialog = new Views.AuthorsManagementView(_context);
            dialog.ShowDialog();
            OnPropertyChanged(nameof(Authors));
        }

        private void ManageGenres(object? parameter)
        {
            var dialog = new Views.GenresManagementView(_context);
            dialog.ShowDialog();
            OnPropertyChanged(nameof(Genres));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}