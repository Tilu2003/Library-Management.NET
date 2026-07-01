using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.ViewModels;

public class MemberViewModel : ViewModelBase
{
    private readonly IMemberService _memberService;

    public ObservableCollection<Member> Members { get; } = new();

    private Member? _selectedMember;
    public Member? SelectedMember
    {
        get => _selectedMember;
        set { SetProperty(ref _selectedMember, value); if (value != null) PopulateForm(value); }
    }

    // Form fields
    private string _formName        = string.Empty;
    private string _formContactInfo = string.Empty;
    private DateTime _formMembershipDate = DateTime.Today;

    public string   FormName           { get => _formName;           set => SetProperty(ref _formName, value); }
    public string   FormContactInfo    { get => _formContactInfo;    set => SetProperty(ref _formContactInfo, value); }
    public DateTime FormMembershipDate { get => _formMembershipDate; set => SetProperty(ref _formMembershipDate, value); }

    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set { SetProperty(ref _searchTerm, value); _ = SearchAsync(); }
    }

    public ICommand LoadCommand   { get; }
    public ICommand AddCommand    { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearCommand  { get; }

    public MemberViewModel(IMemberService memberService)
    {
        _memberService = memberService;
        LoadCommand   = new AsyncRelayCommand(LoadAsync);
        AddCommand    = new AsyncRelayCommand(AddAsync,    () => !IsBusy);
        UpdateCommand = new AsyncRelayCommand(UpdateAsync, () => SelectedMember != null && !IsBusy);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedMember != null && !IsBusy);
        ClearCommand  = new RelayCommand(Clear);
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var members = await _memberService.GetAllMembersAsync();
            Members.Clear();
            foreach (var m in members) Members.Add(m);
            StatusMessage = $"{Members.Count} members loaded.";
        }
        catch (Exception ex) { StatusMessage = ex.Message; }
        finally { IsBusy = false; }
    }

    private async Task SearchAsync()
    {
        var members = await _memberService.SearchMembersAsync(SearchTerm);
        Members.Clear();
        foreach (var m in members) Members.Add(m);
    }

    private async Task AddAsync()
    {
        if (!Validate()) return;
        IsBusy = true;
        try
        {
            var member = BuildFromForm();
            await _memberService.AddMemberAsync(member);
            await LoadAsync();
            Clear(null);
            StatusMessage = "Member added.";
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning); }
        finally { IsBusy = false; }
    }

    private async Task UpdateAsync()
    {
        if (SelectedMember == null || !Validate()) return;
        IsBusy = true;
        try
        {
            var member = BuildFromForm();
            member.Id = SelectedMember.Id;
            member.CreatedAt = SelectedMember.CreatedAt;
            await _memberService.UpdateMemberAsync(member);
            await LoadAsync();
            StatusMessage = "Member updated.";
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning); }
        finally { IsBusy = false; }
    }

    private async Task DeleteAsync()
    {
        if (SelectedMember == null) return;
        var result = MessageBox.Show($"Delete member '{SelectedMember.Name}'?", "Confirm",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;
        IsBusy = true;
        try
        {
            await _memberService.DeleteMemberAsync(SelectedMember.Id);
            await LoadAsync();
            Clear(null);
            StatusMessage = "Member deleted.";
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning); }
        finally { IsBusy = false; }
    }

    private void PopulateForm(Member m)
    {
        FormName = m.Name;
        FormContactInfo = m.ContactInfo;
        FormMembershipDate = m.MembershipDate;
    }

    private void Clear(object? _)
    {
        FormName = FormContactInfo = string.Empty;
        FormMembershipDate = DateTime.Today;
        SelectedMember = null;
    }

    private Member BuildFromForm() => new()
    {
        Name = FormName,
        ContactInfo = FormContactInfo,
        MembershipDate = FormMembershipDate
    };

    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(FormName))        { MessageBox.Show("Name is required.");         return false; }
        if (string.IsNullOrWhiteSpace(FormContactInfo)) { MessageBox.Show("Contact info is required."); return false; }
        return true;
    }
}
