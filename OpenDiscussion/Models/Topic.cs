using System.ComponentModel.DataAnnotations;

namespace OpenDiscussion.Models
{
    public class Topic
    {
        [Key]   
        public int TopicId { get; set; }
        public int? CategoryId { get; set; }
        [Required(ErrorMessage="Numele topicului este obligatoriu")]
        [StringLength(35, ErrorMessage ="Numele topicului nu poate avea mai mult de 35 de caractere")]
        [MinLength(3, ErrorMessage ="Numele topicului trebuie sa contina cel putin 3 caractere")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Descrierea topicului este obligatorie")]
        public string Description { get; set; }
        public virtual ICollection<Discussion>? Discussions { get; set; }
    }
}
