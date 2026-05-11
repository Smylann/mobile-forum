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
    public partial class OwnPostsVM : ViewModelBase
    {
        private readonly consoleClientModel _model;
        private readonly UserVM _userprofile;
        public string Username { get; }

        [ObservableProperty] private int _displayLimit = 10;
        private readonly List<Homepage.PostVM> _fullPosts;   
        public ObservableCollection<Homepage.PostVM>? OwnPosts { get; } = new();

        public OwnPostsVM(consoleClientModel model, UserVM user, string username, IEnumerable<OwnPosts> posts)
        {
            _model = model;
            _userprofile = user;
            Username = username;
            
            // Map the DTOs to ViewModels once
            _fullPosts = posts.Select(p => new Homepage.PostVM(_model, p, this, null, null, null, null)).ToList();
            
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
            var toAdd = _fullPosts.Skip(OwnPosts.Count).Take(DisplayLimit - OwnPosts.Count); //we skip the already loaded ones, and add 10 more

            foreach (var post in toAdd)
            {
                OwnPosts.Add(post);
            }
        }

        [RelayCommand]
        private void Close() => WeakReferenceMessenger.Default.Send(new NavigateToUserMessage(_userprofile.User));
    }
}
