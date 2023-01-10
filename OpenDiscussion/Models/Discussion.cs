using Microsoft.AspNetCore.Mvc.Rendering;
using OpenDiscussion.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenDiscussion.Models
{
    public class Discussion
    {
        [Key]
        public int DiscussionId { get; set; }
        public int? TopicId { get; set; }
        [Required(ErrorMessage ="Titlul discutiei este obligatoriu")]
        [StringLength(120, ErrorMessage ="Titlul discutiei nu poate avea mai mult de 120 de caractere")]
        [MinLength(3, ErrorMessage ="Titlul discutiei trebuie sa continta cel putin 3 caractere")]
        public string Title { get; set; }
        public DateTime Date { get; set; }
        [Required(ErrorMessage ="Continutul discutiei este obligatoriu")]
        [MinLength(20, ErrorMessage ="Continutul discutiei trebuie sa contina cel putin 20 de caractere")]
        public string Content { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        [NotMapped]
        public int CommentsCount { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? AllTopics { get; set; }
    }
}
