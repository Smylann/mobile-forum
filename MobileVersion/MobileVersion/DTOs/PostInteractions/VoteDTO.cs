namespace MobileVersion.DTOs.PostInteractions;

public class VoteDTO
{
    public int userId { get; set; }
    public int postId { get; set; }
    public bool isUpvote { get; set; }
}