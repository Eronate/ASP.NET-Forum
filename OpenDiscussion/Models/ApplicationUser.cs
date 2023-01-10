using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenDiscussion.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; }
        public DateTime? DateOfCreation { get; set; }
        [NotMapped]
        public virtual ICollection<Comment>? Comments { get; set; }
        [NotMapped]
        public virtual ICollection<Discussion>? Discussions { get; set; }
        public virtual Profile? Profile { get; set; }   
        public int? CommentCount { get; set; }
        public int? DiscussionCount { get; set; }
        [NotMapped]
        public IEnumerable <SelectListItem>? AllRoles { get; set; }
    }
}
