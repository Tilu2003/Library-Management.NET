using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.ViewModels;

/// <summary>
/// ViewModel for Book management.
/// Follows MVVM: View binds to properties/commands here; no code-behind logic.
/// </summary>
public class BookViewModel : ViewModelBase
{
    private readonly IBookService _bookService;

    // ── Observable collections (auto-update the UI when changed) ──────────────
    public ObservableCollection<Book> Books { get; } = new();

    // ── Selected book ─────────────────────────────────────────────────────────
    private Book? _selectedBook;
    public Book? SelectedBook
    {
        get => _selectedBook;
        set
        {
            SetProperty(ref _selectedBook, value);
            // Populate edit form
            if (value != null) PopulateForm(value);
        }
    }

    // ── Form Fields ───────────────────────────────────────────────────────────
    private string _formTitle   = string.Empty;
    private string _formAuthor  = string.Empty;
    private string _formIsbn    = string.Empty;
    private string _formGenre   = string.Empty;
    private bool   _formIsAvailable = true;

    public string FormTitle    { get => _formTitle;    set => SetProperty(ref _formTitle, value); }
    public string FormAuthor   { get => _formAuthor;   set => SetProperty(ref _formAuthor, value); }
    public string FormISBN     { get => _formIsbn;     set => SetProperty(ref _formIsbn, value); }
    public string FormGenre    { get => _formGenre;    set => SetProperty(ref _formGenre, value); }
    public bool   FormIsAvailable { get => _formIsAvailable; set => SetProperty(ref _formIsAvailable, value); }

    // ── Search ────────────────────────────────────────────────────────────────
    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            SetProperty(ref _searchTerm, value);
            _ = SearchAsync();
        }
    }

    // ── Commands ──────────────────────────────────────────────────────────────
    public ICommand LoadCommand   { get; }
    public ICommand AddCommand    { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearCommand  { get; }

    public BookViewModel(IBookService bookService)
    {
        _bookService = bookService;

        LoadCommand   = new AsyncRelayCommand(LoadBooksAsync);
        AddCommand    = new AsyncRelayCommand(AddBookAsync,    () => !IsBusy);
        UpdateCommand = new AsyncRelayCommand(UpdateBookAsync, () => SelectedBook != null && !IsBusy);
        DeleteCommand = new AsyncRelayCommand(DeleteBookAsync, () => SelectedBook != null && !IsBusy);
        ClearCommand  = new RelayCommand(ClearForm);
    }

    private async Task LoadBooksAsync()
    {
        IsBusy = true;
        try
        {
            var books = await _bookService.GetAllBooksAsync();
            Books.Clear();
            foreach (var b in books) Books.Add(b);
            StatusMessage = $"{Books.Count} books loaded.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading books: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    private async Task SearchAsync()
    {
        var books = await _bookService.SearchBooksAsync(SearchTerm);
        Books.Clear();
        foreach (var b in books) Books.Add(b);
    }

    private async Task AddBookAsync()
    {
        if (!ValidateForm()) return;
        IsBusy = true;
        try
        {
            var book = BuildBookFromForm();
            await _bookService.AddBookAsync(book);
            await LoadBooksAsync();
            ClearForm(null);
            StatusMessage = "Book added successfully.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally { IsBusy = false; }
    }

    private async Task UpdateBookAsync()
    {
        if (SelectedBook == null || !ValidateForm()) return;
        IsBusy = true;
        try
        {
            var book = BuildBookFromForm();
            book.Id = SelectedBook.Id;
            book.CreatedAt = SelectedBook.CreatedAt;
            await _bookService.UpdateBookAsync(book);
            await LoadBooksAsync();
            StatusMessage = "Book updated successfully.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally { IsBusy = false; }
    }

    private async Task DeleteBookAsync()
    {
        if (SelectedBook == null) return;

        var result = MessageBox.Show(
            $"Delete '{SelectedBook.Title}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        IsBusy = true;
        try
        {
            await _bookService.DeleteBookAsync(SelectedBook.Id);
            await LoadBooksAsync();
            ClearForm(null);
            StatusMessage = "Book deleted.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally { IsBusy = false; }
    }

    private void PopulateForm(Book book)
    {
        FormTitle       = book.Title;
        FormAuthor      = book.Author;
        FormISBN        = book.ISBN;
        FormGenre       = book.Genre;
        FormIsAvailable = book.IsAvailable;
    }

    private void ClearForm(object? _)
    {
        FormTitle = FormAuthor = FormISBN = FormGenre = string.Empty;
        FormIsAvailable = true;
        SelectedBook = null;
    }

    private Book BuildBookFromForm() => new()
    {
        Title       = FormTitle,
        Author      = FormAuthor,
        ISBN        = FormISBN,
        Genre       = FormGenre,
        IsAvailable = FormIsAvailable
    };

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(FormTitle))  { MessageBox.Show("Title is required.");  return false; }
        if (string.IsNullOrWhiteSpace(FormAuthor)) { MessageBox.Show("Author is required."); return false; }
        if (string.IsNullOrWhiteSpace(FormISBN))   { MessageBox.Show("ISBN is required.");   return false; }
        return true;
    }
}
