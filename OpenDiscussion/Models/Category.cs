using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;

namespace OpenDiscussion.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required(ErrorMessage ="Numele categoriei este obligatoriu")]
        [StringLength(35, ErrorMessage ="Numele categoriei nu poate avea mai mult de 35 de caractere")]
        [MinLength(3, ErrorMessage ="Numele categoriei trebuie sa contina cel putin 3 caractere")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Descrierea categoriei este obligatorie")]
        public string Description { get; set; }

        public virtual ICollection<Topic>? Topics { get; set; }

    }
}
