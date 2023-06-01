using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StarWars.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Planet
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public virtual ICollection<Character>? Character { get; set; }
    }
}