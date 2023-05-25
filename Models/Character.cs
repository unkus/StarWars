using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace StarWars.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Character
    {
        [ModelBinder(Name = "Id")]
        public int Id { get; set; }
        
        [Display(Name = "Имя персонажа")]
        public string Name { get; set; }
        
        [Display(Name = "Имя (в оригинале)")]
        public string OriginalName { get; set; }

        [Display(Name = "Дата рождения")]
        public int BirthDate { get; set; }
        
        [Display(Name = "Планета")]
        [ForeignKey("Planet")]
        public int PlanetID { get; set; }

        [Display(Name = "Пол")]
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
        
        [Display(Name = "Раса")]
        [ForeignKey("Race")]
        public int RaceID { get; set; }
        
        [Display(Name = "Рост")]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:N2} м", ApplyFormatInEditMode = false)]
        public decimal Height { get; set; }
        
        [Display(Name = "Цвет волос")]
        [ForeignKey("HairColor")]
        public int HairColorID { get; set; }
        
        [Display(Name = "Цвет глаз")]
        [ForeignKey("EyeColor")]
        public int EyeColorID { get; set; }
        
        [Display(Name = "Описание")]
        public string History { get; set; }

        public virtual Planet Planet { get; set; }
        public virtual Race Race { get; set; }
        public virtual HairColor HairColor { get; set; }
        public virtual EyeColor EyeColor { get; set; }

        [Display(Name = "Фильмы")]
        [BindNever]
        public virtual ICollection<Movie> Movies { get; set; }
    }

}