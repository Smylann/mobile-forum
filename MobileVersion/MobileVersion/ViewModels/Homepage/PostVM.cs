using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MobileVersion.DTOs;
using MobileVersion.DTOs.Homepage;
using MobileVersion.DTOs.PostInteractions;
using MobileVersion.DTOs.UserInterface;
using MobileVersion.Messages;
using MobileVersion.Model;

namespace MobileVersion.ViewModels.Homepage
{
    // 1. Add IRecipient
    public partial class PostVM : ViewModelBase, IRecipient<PostUpdatedMessage>
    {
        private readonly consoleClientModel _model;
        private readonly User.OwnPostsVM? _ownposts;
        private readonly User.OwnCommentsVM? _owncomms;
        private readonly User.DislikedPostsVM? _dposts;
        private readonly User.LikedPostsVM? _lposts;
        private readonly User.FavoritePostsVM? _fposts;

        [ObservableProperty] private DisplayAllPostsDTO _post;
        [ObservableProperty] private int _displayLimit = 10;
        private List<GetCommentsFromPost> _comments;
        [ObservableProperty] private ObservableCollection<GetCommentsFromPost>? _displayComments = new();
////////////////////////////////////
        public List<OwnComments> UserOwnComments =>
            (Post as PostsFromOwnComment)?.OwnComments ?? new List<OwnComments>();
////////////////////////////////////

        [ObservableProperty] private int _votes;
        [ObservableProperty] private string _upvotecolor;
        [ObservableProperty] private string _downvotecolor;
        [ObservableProperty] private string _favorited;
        [ObservableProperty] private string _commentText;
        [ObservableProperty] private string _reportReason;
        [ObservableProperty] private string _repStatus;
        [ObservableProperty] private string _voteStatus;
        [ObservableProperty] private bool _isCommentingOpen;
        [ObservableProperty] private bool _isReportOpen;
////////////////////////////////////
        public bool IsLoggedIn => _model.CurrentUser != null;
////////////////////////////////////
        [ObservableProperty] private bool _notReported;
        [ObservableProperty] private string? _imageUrl;
////////////////////////////////////
        [ObservableProperty] private bool _isConfirmOpen;
        [ObservableProperty] private string _confirmMessage = string.Empty;

        private TaskCompletionSource<bool>? _confirmTcs;

        //Confirmation for delete
        public async Task<bool> ConfirmAsync(string message)
        {
            ConfirmMessage = message;
            IsConfirmOpen = true;

            _confirmTcs = new TaskCompletionSource<bool>();
            
            return await _confirmTcs.Task;
        }

        [RelayCommand]
        private void ConfirmYes()
        {
            IsConfirmOpen = false;
            _confirmTcs?.TrySetResult(true);
        }

        [RelayCommand]
        private void ConfirmNo()
        {
            IsConfirmOpen = false;
            _confirmTcs?.TrySetResult(false);
        }
////////////////////////////////////
        public PostVM(consoleClientModel model,
            DisplayAllPostsDTO post,
            User.OwnPostsVM? ownposts = null,
            User.OwnCommentsVM? comms = null,
            User.DislikedPostsVM? dposts = null,
            User.LikedPostsVM? lposts = null,
            User.FavoritePostsVM? fposts = null)
        {
            _model = model;
            _post = post;
            _ownposts = ownposts;
            _owncomms = comms;
            _dposts = dposts;
            _lposts = lposts;
            _fposts = fposts;
            _comments = new();
            _votes = post.Votes;
            _upvotecolor = "Transparent";
            _downvotecolor = "Transparent";
            _favorited = "Transparent";
            _isCommentingOpen = false;
            _isReportOpen = false;
            _notReported = true;
            CommentText = string.Empty;
            ReportReason = string.Empty;
            RepStatus = string.Empty;
            VoteStatus = string.Empty;
            ImageUrl = !string.IsNullOrEmpty(post.ImagePath)
                ? $"{App.GetBackendUrl().TrimEnd('/')}{post.ImagePath}"
                : null;
            _ = InitializeComments(); //loads comments on new post open
            _ = InitializeColors(); //loads interaction colors on new post open
            _ = InitializeReport(); //loads report status on new post open
            _ = InitializeVotes();

            WeakReferenceMessenger.Default.Register(this);
        }


        // Sends NavigateToPostMessage → caught by MainViewModel.Receive(NavigateToPostMessage)
        [RelayCommand]
        private void Select()
        {
            DisplayLimit = 10;
            DisplayComments?.Clear();
            _ = InitializeComments();
            WeakReferenceMessenger.Default.Send(new NavigateToPostMessage(this));
        }

        // Sends GoBackMessage → caught by MainViewModel.Receive(GoBackMessage)
        [RelayCommand]
        private void Close()
        {
            if (_ownposts != null) WeakReferenceMessenger.Default.Send(new NavigateToOwnPostsMessage(_ownposts));
            else if (_owncomms != null)
                WeakReferenceMessenger.Default.Send(new NavigateToOwnCommentsMessage(_owncomms));
            else if (_dposts != null) WeakReferenceMessenger.Default.Send(new NavigateToDislikedPostsMessage(_dposts));
            else if (_lposts != null) WeakReferenceMessenger.Default.Send(new NavigateToLikedPostsMessage(_lposts));
            else if (_fposts != null) WeakReferenceMessenger.Default.Send(new NavigateToFavoritesMessage(_fposts));
            else WeakReferenceMessenger.Default.Send(new GoBackMessage());
        }

        [RelayCommand]
        private void OpenCommenting() => IsCommentingOpen = !IsCommentingOpen;

        [RelayCommand]
        private void OpenReporting() => IsReportOpen = !IsReportOpen;

        [RelayCommand]
        private async Task UpVote()
        {
            if (_model.CurrentUser == null) return;
            await _model.votePost(new VoteDTO
                { postId = Post.PostID, userId = _model.CurrentUser.UserID, isUpvote = true });

            if (Upvotecolor == "Green")
            {
                Upvotecolor = "Transparent";
                Votes--; // Removing an upvote
                _lposts?.LikedPosts.Remove(this);
            }
            else
            {
                if (Downvotecolor == "Red") Votes++; // Removing the downvote penalty

                Upvotecolor = "Green";
                Downvotecolor = "Transparent";
                Votes++; // Adding the upvote
                _dposts?.DislikedPosts.Remove(this);
            }

            NotifySync();
            InitializeVotes();
        }

        [RelayCommand]
        private async Task DownVote()
        {
            if (_model.CurrentUser == null) return;
            await _model.votePost(new VoteDTO
                { postId = Post.PostID, userId = _model.CurrentUser.UserID, isUpvote = false });

            if (Downvotecolor == "Red")
            {
                Downvotecolor = "Transparent";
                Votes++; // Removing a dislike
                _dposts?.DislikedPosts.Remove(this);
            }
            else
            {
                if (Upvotecolor == "Green") Votes--; // Removing the downvote penalty

                Downvotecolor = "Red";
                Upvotecolor = "Transparent";
                Votes--; // Adding the dislike
                _lposts?.LikedPosts.Remove(this);
            }

            NotifySync();
            InitializeVotes();
        }

        [RelayCommand]
        private async Task Favoriting()
        {
            if (_model.CurrentUser == null) return;
            await _model.favouritePosts(new FavouritePostDTO
                { postId = Post.PostID, userId = _model.CurrentUser.UserID });

            if (Favorited == "Yellow")
            {
                Favorited = "Transparent";
                _fposts?.FavoritePosts.Remove(this);
            }
            else
            {
                Favorited = "Yellow";
            }

            NotifySync();
        }

        [RelayCommand]
        private async Task DeleteOwnPost()
        {
            if (_model.CurrentUser == null) return;
            bool isSure = await ConfirmAsync("Are you sure you want to delete this post?");
            if (!isSure) return;
            try
            {
                await _model.deleteOwnPost(new DeleteOwnPostDTO
                    { postid = Post.PostID, userId = _model.CurrentUser.UserID });
                _ownposts?.OwnPosts.Remove(this);
            }
            catch
            {
            }
        }

        [RelayCommand]
        private async Task DeletePost()
        {
            if (_model.CurrentUser == null && _model.CurrentUser.Role != "Admin") return;
            bool isSure = await ConfirmAsync("Are you sure you want to delete this post?");
            if (!isSure) return;
            try
            {
                await _model.deletePosts(Post.PostID);
                _ownposts?.OwnPosts.Remove(this);
            }
            catch
            {
            }
        }

        [RelayCommand]
        private async Task Commenting()
        {
            if (_model.CurrentUser == null) return;
            try
            {
                await _model.comment(new CommentDTO
                    { userID = _model.CurrentUser.UserID, postID = Post.PostID, commentcontent = CommentText });
                var comment = new GetCommentsFromPost
                {
                    commentcontent = CommentText, userID = _model.CurrentUser.UserID,
                    username = _model.CurrentUser.Username, commentcreated_at = DateTime.Now
                };
                _comments.Add(comment);
                DisplayComments.Add(comment);
                IsCommentingOpen = false;
                CommentText = string.Empty;
            }
            catch
            {
            }
        }


        [RelayCommand]
        private async Task Report()
        {
            if (_model.CurrentUser != null && NotReported)
            {
                try
                {
                    await _model.createReport(new ReportDTO
                    {
                        userID = _model.CurrentUser.UserID, postID = Post.PostID, reportreason = ReportReason,
                        reportcreated_at = DateTime.Now
                    });
                    IsReportOpen = false;
                    ReportReason = string.Empty;
                    NotReported = false;
                    RepStatus = "Pending";
                    NotifySync();
                }
                catch
                {
                }
            }
        }

        //refetch datas when opening a new instances of the same post
        private async Task InitializeColors()
        {
            if (_model.CurrentUser == null) return;

            var liked = await _model.likedposts(_model.CurrentUser.UserID);
            if (liked.Any(x => x.PostID == Post.PostID)) Upvotecolor = "Green";

            var disliked = await _model.dislikedposts(_model.CurrentUser.UserID);
            if (disliked.Any(x => x.PostID == Post.PostID)) Downvotecolor = "Red";

            var favs = await _model.favorites(_model.CurrentUser.UserID);
            if (favs.Any(x => x.PostID == Post.PostID)) Favorited = "Yellow";
        }

        private async Task InitializeComments()
        {
            try
            {
                _comments.Clear();
                _comments = await _model.fetchcomments(Post.PostID);
                DisplayComments?.Clear();
                UpdateVisibleComments();
            }
            catch
            {
            }
        }

        [RelayCommand]
        public void LoadMore()
        {
            if (DisplayLimit < _comments.Count) //if we only have 9, nothing gets added
            {
                DisplayLimit += 10;
                UpdateVisibleComments();
            }
        }

        private void UpdateVisibleComments()
        {
            DisplayComments?.Clear();
            foreach (var comment in Enumerable.Take(_comments, (int)DisplayLimit))
            {
                DisplayComments?.Add(comment);
            }
        }

        private async Task InitializeReport()
        {
            if (_model.CurrentUser == null) return;

            List<OwnReports> reps = await _model.reports(_model.CurrentUser.UserID);
            OwnReports? existingReport = reps.FirstOrDefault(x => x.PostID == Post.PostID);

            if (existingReport != null)
            {
                NotReported = false;
                RepStatus = existingReport.ReportStatus;
            }
        }

        private async Task InitializeVotes()
        {
            if (Votes < 0)
            {
                VoteStatus = "💀";
            }
            else if (Votes > 1000)
            {
                VoteStatus = "🚀";
            }
            else
            {
                VoteStatus = "🌟";
            }
        }


        /***************************
         *                         *
         *                         *
         *     Real-Time changes   *
         *        (optional)       *
         *                         *
         *                         *
         ***************************/

        // Helper to notify other instances of interaction
        private void NotifySync()
        {
            WeakReferenceMessenger.Default.Send(new PostUpdatedMessage(
                Post.PostID,
                Upvotecolor,
                Downvotecolor,
                Favorited,
                Votes,
                DisplayComments,
                NotReported,
                RepStatus));
        }

        // Handles NotifySync()
        public void Receive(PostUpdatedMessage message)
        {
            if (message.PostId == Post.PostID)
            {
                Upvotecolor = message.UpvoteColor;
                Downvotecolor = message.DownvoteColor;
                Favorited = message.FavoriteColor;
                Votes = message.Votes;
                DisplayComments = message.comments;
                NotReported = message.reported;
                RepStatus = message.reportstatus;
            }
        }
    }
}