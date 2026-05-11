using System.Collections.ObjectModel;
using MobileVersion.DTOs;
using MobileVersion.DTOs.PostInteractions;

namespace MobileVersion.Messages
{
    public record PostUpdatedMessage(
        int PostId, 
        string UpvoteColor, 
        string DownvoteColor, 
        string FavoriteColor, 
        int Votes, 
        ObservableCollection<GetCommentsFromPost> comments, 
        bool reported,
        string reportstatus
        );
    public record PostCreatedMessage();
}