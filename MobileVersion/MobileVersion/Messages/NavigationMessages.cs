using MobileVersion.DTOs;
using MobileVersion.DTOs.Homepage;
using MobileVersion.ViewModels;
using MobileVersion.ViewModels.Admin;
using CreatePostVM = MobileVersion.ViewModels.Homepage.CreatePostVM;
using DislikedPostsVM = MobileVersion.ViewModels.User.DislikedPostsVM;
using FavoritePostsVM = MobileVersion.ViewModels.User.FavoritePostsVM;
using LikedPostsVM = MobileVersion.ViewModels.User.LikedPostsVM;
using LoginVM = MobileVersion.ViewModels.Homepage.LoginVM;
using OwnCommentsVM = MobileVersion.ViewModels.User.OwnCommentsVM;
using OwnPostsVM = MobileVersion.ViewModels.User.OwnPostsVM;
using PostVM = MobileVersion.ViewModels.Homepage.PostVM;
using SettingsVM = MobileVersion.ViewModels.User.SettingsVM;

namespace MobileVersion.Messages;

public record GoBackMessage();
public record NavigateToPostMessage(PostVM PostVM);
public record NavigateToUserMessage(DisplayAllUserDTO User);
public record NavigateToOwnPostsMessage(OwnPostsVM ownpostsvm);
public record NavigateToOwnCommentsMessage(OwnCommentsVM owncommentsvm);
public record NavigateToLikedPostsMessage(LikedPostsVM likedpostsvm);
public record NavigateToDislikedPostsMessage(DislikedPostsVM dislikedpostsvm);
public record NavigateToFavoritesMessage(FavoritePostsVM favoritepostsvm);
public record NavigateToSettingsMessage(SettingsVM settingsvm);
public record NavigateToAboutUsMessage();
public record NavigateToLoginMessage(LoginVM loginvm);
public record NavigateToCreatePostMessage(CreatePostVM createvm);
public record NavigateToDeleteUsersMessage(DeleteUsersVM delusersvm);
public record NavigateToDeletePostsMessage(DeletePostsVM delpostsvm);
public record NavigateToDeleteCommentsMessage(DeleteCommentsVM delcommentsvm);
public record NavigateToCreateCategoryMessage(CreateCategoryVM createcatvm);
public record NavigateToDeleteCategoryMessage(DeleteCategoryVM delcatvm);



