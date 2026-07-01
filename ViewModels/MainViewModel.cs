namespace LibraryManagementSystem.ViewModels;

public class MainViewModel : ViewModelBase
{
    public BookViewModel BookVM { get; }
    public MemberViewModel MemberVM { get; }
    public TransactionViewModel TransactionVM { get; }

    private int _selectedTabIndex;
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            SetProperty(ref _selectedTabIndex, value);
            if (value == 0) BookVM.LoadCommand.Execute(null);
            else if (value == 1) MemberVM.LoadCommand.Execute(null);
            else if (value == 2) TransactionVM.LoadCommand.Execute(null);
        }
    }

    public MainViewModel(
        BookViewModel bookVM,
        MemberViewModel memberVM,
        TransactionViewModel transactionVM)
    {
        BookVM = bookVM;
        MemberVM = memberVM;
        TransactionVM = transactionVM;
    }
}