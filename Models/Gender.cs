
using System.ComponentModel.DataAnnotations;

namespace StarWars.Models
{
    public enum Gender
    {
        [Display(Name = "Мужчина")]
        Male = 1,
        
        [Display(Name = "Женщина")]
        Female = 2,
        
        // это может быть актуально для некоторых форм жизни
        [Display(Name = "Другой")]
        Other = 3
    }
}