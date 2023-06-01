using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StarWars.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Character
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string OriginalName { get; set; }

        public int BirthDate { get; set; }
        
        [ForeignKey("Planet")]
        public int PlanetID { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
        
        [ForeignKey("Race")]
        public int RaceID { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Height { get; set; }
        
        [ForeignKey("HairColor")]
        public int HairColorID { get; set; }
        
        [ForeignKey("EyeColor")]
        public int EyeColorID { get; set; }
        
        public string History { get; set; }

        public virtual Planet Planet { get; set; }
        public virtual Race Race { get; set; }
        public virtual HairColor HairColor { get; set; }
        public virtual EyeColor EyeColor { get; set; }
        public virtual ICollection<Movie> Movies { get; set; }
    }

}