using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StarWars.Models
{
    [Index(nameof(Title), IsUnique = true)]
    public class Movie
    {
        public int Id { get; set; }
        
        [Display(Name = "Название")]
        public string Title { get; set; }

        public virtual ICollection<Character>? Character { get; set; }
    }
}