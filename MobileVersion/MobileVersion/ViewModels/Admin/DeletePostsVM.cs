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

public partial class DeletePostsVM : ViewModelBase
{
    private readonly consoleClientModel _model;
    private readonly AdminPanelVM _userprofile;
    
    [ObservableProperty] private int _displayLimit = 10;
    public List<Homepage.PostVM> _fullPosts;
    public ObservableCollection<Homepage.PostVM>? Posts { get; } = new();
    public DeletePostsVM(consoleClientModel model,AdminPanelVM user, IEnumerable<DisplayAllPostsDTO> posts)
    {
        _model = model;
        _userprofile = user;
        _fullPosts = posts.Select(p => new Homepage.PostVM(_model, p, null,null, null, null, null)).ToList();
        UpdateVisiblePosts();
    }
    [RelayCommand]
    public void LoadMore()
    {
        if (DisplayLimit < _fullPosts.Count) //if we only have 9, nothing gets added
        {
            DisplayLimit += 10;
            UpdateVisiblePosts();
        }
    }
    private void UpdateVisiblePosts()
    {
        var toAdd = _fullPosts.Skip(Posts.Count).Take(DisplayLimit - Posts.Count); //we skip the already loaded ones, and add 10 more

        foreach (var post in toAdd)
        {
            Posts.Add(post);
        }
    }
    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new NavigateToUserMessage(_userprofile.User));
}