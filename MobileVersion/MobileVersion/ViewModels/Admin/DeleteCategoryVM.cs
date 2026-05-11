using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MobileVersion.DTOs;
using MobileVersion.DTOs.Homepage;
using MobileVersion.Messages;
using MobileVersion.Model;
using MobileVersion.ViewModels.User;

namespace MobileVersion.ViewModels.Admin;

public partial class DeleteCategoryVM : ViewModelBase
{
    private readonly consoleClientModel _model;
    private readonly AdminPanelVM _userprofile;
    
    [ObservableProperty] private int _displayLimit = 10;
    public List<CategoryDTO> _fullCats;
    public ObservableCollection<CategoryDTO>? Cats { get; } = new();
    public DeleteCategoryVM(consoleClientModel model,AdminPanelVM user, IEnumerable<CategoryDTO> cats)
    {
        _model = model;
        _userprofile = user;
        _fullCats = cats.ToList();
        UpdateVisibleCats();
    }
    [RelayCommand]
    public void LoadMore()
    {
        if (DisplayLimit < _fullCats.Count) //if we only have 9, nothing gets added
        {
            DisplayLimit += 10;
            UpdateVisibleCats();
        }
    }
    private void UpdateVisibleCats()
    {
        var toAdd = _fullCats.Skip(Cats.Count).Take(DisplayLimit - Cats.Count); //we skip the already loaded ones, and add 10 more

        foreach (var cat in toAdd)
        {
            Cats.Add(cat);
        }
    }
    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new NavigateToUserMessage(_userprofile.User));
}