using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MobileVersion.DTOs;
using MobileVersion.DTOs.Homepage;
using MobileVersion.Messages;
using MobileVersion.Model;

namespace MobileVersion.ViewModels.Admin;

public partial class AdminPanelVM : ViewModelBase
{
    private readonly consoleClientModel _model;

        public AdminPanelVM(DisplayAllUserDTO user, consoleClientModel model)
        {
            _user = user;
            _model = model;
        }

        [ObservableProperty] private DisplayAllUserDTO _user;

        [RelayCommand]
        private void Close() => WeakReferenceMessenger.Default.Send(new GoBackMessage());

        [RelayCommand]
        private void Select() => WeakReferenceMessenger.Default.Send(new NavigateToUserMessage(User));
        

        [RelayCommand]
        private async Task ToDeleteUsers()
        {
            try
            {
                var users = await _model.getallusers();
                WeakReferenceMessenger.Default.Send(new NavigateToDeleteUsersMessage(new DeleteUsersVM(_model,this, users ?? new())));
            }
            catch { }
        }
        [RelayCommand]
        private async Task ToDeletePosts()
        {
            try
            {
                var posts = await _model.getallposts();
                WeakReferenceMessenger.Default.Send(new NavigateToDeletePostsMessage(new DeletePostsVM(_model, this, posts ?? new())));
            }
            catch { }
        }
        [RelayCommand]
        private async Task ToDeleteComments()
        {
            try
            {
                var lposts = await _model.likedposts(User.UserID);
                WeakReferenceMessenger.Default.Send(new NavigateToLikedPostsMessage(new User.LikedPostsVM(_model,this, User.Username, lposts ?? new())));
            }
            catch { }
        }
        [RelayCommand]
        private async Task ToCreateCategory()
        {
            try
            {
                var dposts = await _model.dislikedposts(User.UserID);
                WeakReferenceMessenger.Default.Send(new NavigateToDislikedPostsMessage(new User.DislikedPostsVM(_model, this, User.Username, dposts ?? new())));
            }
            catch { }
        }
        [RelayCommand]
        private async Task ToDeleteCategory()
        {
            try
            {
                var cats = await _model.getallcats();
                WeakReferenceMessenger.Default.Send(new NavigateToDeleteCategoryMessage(new DeleteCategoryVM(_model, this, cats ?? new())));
            }
            catch { }
        }
}