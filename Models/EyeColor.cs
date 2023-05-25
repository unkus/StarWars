using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StarWars.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class EyeColor
    {
        public int Id { get; set; }
        
        [Display(Name = "Цвет глаз")]
        public string Name { get; set; }

        public virtual ICollection<Character>? Character { get; set; }
    }
}