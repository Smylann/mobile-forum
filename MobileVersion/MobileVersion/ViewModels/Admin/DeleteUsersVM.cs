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

public partial class DeleteUsersVM: ViewModelBase
{
    private readonly consoleClientModel _model;
    private readonly AdminPanelVM _userprofile;
    
    [ObservableProperty] private int _displayLimit = 10;
    public List<UserVM> _fullUsers;
    public ObservableCollection<UserVM>? Users { get; } = new();
    public DeleteUsersVM(consoleClientModel model,AdminPanelVM user, IEnumerable<DisplayAllUserDTO> users)
    {
        _model = model;
        _userprofile = user;
        _fullUsers = users.Select(p => new UserVM(p, _model)).ToList();
        UpdateVisibleUsers();
    }
    [RelayCommand]
    public void LoadMore()
    {
        if (DisplayLimit < _fullUsers.Count) //if we only have 9, nothing gets added
        {
            DisplayLimit += 10;
            UpdateVisibleUsers();
        }
    }
    private void UpdateVisibleUsers()
    {
        var toAdd = _fullUsers.Skip(Users.Count).Take(DisplayLimit - Users.Count); //we skip the already loaded ones, and add 10 more

        foreach (var user in toAdd)
        {
            Users.Add(user);
        }
    }
    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new NavigateToUserMessage(_userprofile.User));
}