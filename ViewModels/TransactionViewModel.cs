using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.ViewModels;

public class TransactionViewModel : ViewModelBase
{
    private readonly ITransactionService _txService;
    private readonly IBookService        _bookService;
    private readonly IMemberService      _memberService;

    public ObservableCollection<BorrowTransaction> Transactions { get; } = new();
    public ObservableCollection<Book>   AvailableBooks { get; } = new();
    public ObservableCollection<Member> AllMembers     { get; } = new();

    private BorrowTransaction? _selectedTransaction;
    public BorrowTransaction? SelectedTransaction
    {
        get => _selectedTransaction;
        set => SetProperty(ref _selectedTransaction, value);
    }

    private Book? _selectedBook;
    public Book? SelectedBook { get => _selectedBook; set => SetProperty(ref _selectedBook, value); }

    private Member? _selectedMember;
    public Member? SelectedMember { get => _selectedMember; set => SetProperty(ref _selectedMember, value); }

    private DateTime _dueDate = DateTime.Today.AddDays(14);
    public DateTime DueDate { get => _dueDate; set => SetProperty(ref _dueDate, value); }

    private bool _showActiveOnly;
    public bool ShowActiveOnly
    {
        get => _showActiveOnly;
        set { SetProperty(ref _showActiveOnly, value); _ = LoadAsync(); }
    }

    public ICommand LoadCommand   { get; }
    public ICommand BorrowCommand { get; }
    public ICommand ReturnCommand { get; }

    public TransactionViewModel(
        ITransactionService txService,
        IBookService bookService,
        IMemberService memberService)
    {
        _txService     = txService;
        _bookService   = bookService;
        _memberService = memberService;

        LoadCommand   = new AsyncRelayCommand(LoadAsync);
        BorrowCommand = new AsyncRelayCommand(BorrowAsync, () => SelectedBook != null && SelectedMember != null && !IsBusy);
        ReturnCommand = new AsyncRelayCommand(ReturnAsync, () => SelectedTransaction != null && !SelectedTransaction.IsReturned && !IsBusy);
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var txTask     = ShowActiveOnly ? _txService.GetActiveTransactionsAsync() : _txService.GetAllTransactionsAsync();
            var booksTask  = _bookService.GetAvailableBooksAsync();
            var memberTask = _memberService.GetAllMembersAsync();

            await Task.WhenAll(txTask, booksTask, memberTask);

            Transactions.Clear();
            foreach (var t in txTask.Result) Transactions.Add(t);

            AvailableBooks.Clear();
            foreach (var b in booksTask.Result) AvailableBooks.Add(b);

            AllMembers.Clear();
            foreach (var m in memberTask.Result) AllMembers.Add(m);

            StatusMessage = $"{Transactions.Count} transactions loaded.";
        }
        catch (Exception ex) { StatusMessage = ex.Message; }
        finally { IsBusy = false; }
    }

    private async Task BorrowAsync()
    {
        if (SelectedBook == null || SelectedMember == null) return;
        IsBusy = true;
        try
        {
            await _txService.BorrowBookAsync(SelectedBook.Id, SelectedMember.Id, DueDate);
            await LoadAsync();
            SelectedBook = null;
            SelectedMember = null;
            DueDate = DateTime.Today.AddDays(14);
            StatusMessage = "Book borrowed successfully.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Cannot Borrow", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally { IsBusy = false; }
    }

    private async Task ReturnAsync()
    {
        if (SelectedTransaction == null) return;
        var result = MessageBox.Show(
            $"Return '{SelectedTransaction.Book?.Title}'?",
            "Confirm Return", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;

        IsBusy = true;
        try
        {
            await _txService.ReturnBookAsync(SelectedTransaction.Id);
            await LoadAsync();
            StatusMessage = "Book returned.";
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning); }
        finally { IsBusy = false; }
    }
}
