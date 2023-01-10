using System.ComponentModel.DataAnnotations;

namespace OpenDiscussion.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set;}
        public int? DiscussionId { get; set;}
        [Required(ErrorMessage ="Continutul comentariului este obligatoriu")]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
