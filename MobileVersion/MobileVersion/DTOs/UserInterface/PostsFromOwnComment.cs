using System.Collections.Generic;
using MobileVersion.DTOs.Homepage;

namespace MobileVersion.DTOs.UserInterface
{
    public class PostsFromOwnComment : DisplayAllPostsDTO
    {
        public List<OwnComments> OwnComments { get; set; }
    }
}
