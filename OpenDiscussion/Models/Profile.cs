using System.ComponentModel.DataAnnotations;

namespace OpenDiscussion.Models
{
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }
        public string? ApplicationUserId { get; set; }
        public string? Description { get; set; }

        public string? Avatar { get; set; }   
    }
}
