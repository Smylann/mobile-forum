using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MobileVersion.DTOs;
using MobileVersion.DTOs.UserInterface;
using MobileVersion.Messages;
using MobileVersion.Model;

namespace MobileVersion.ViewModels.User
{
    public partial class DislikedPostsVM : ViewModelBase
    {
        private readonly consoleClientModel _model;
        private readonly UserVM _userprofile;
        public string Username { get; } //you need this as a constructor parameter, because we get the value of this from the uservm's username
        [ObservableProperty] private int _displayLimit = 10;
        public List<Homepage.PostVM> _fullPosts;
        public ObservableCollection<Homepage.PostVM>? DislikedPosts { get; } = new();

        public DislikedPostsVM(consoleClientModel model, UserVM user, string username, IEnumerable<DislikedPosts> posts)
        {
            _model = model;
            _userprofile = user;
            Username = username;
            _fullPosts = posts.Select(p => new Homepage.PostVM(_model, p, null,null, this, null, null)).ToList();
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
            var toAdd = _fullPosts.Skip(DislikedPosts.Count).Take(DisplayLimit - DislikedPosts.Count); //we skip the already loaded ones, and add 10 more

            foreach (var post in toAdd)
            {
                DislikedPosts.Add(post);
            }
        }
        [RelayCommand]
        private void Close() => WeakReferenceMessenger.Default.Send(new NavigateToUserMessage(_userprofile.User));
    }
}
