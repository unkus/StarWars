using System.ComponentModel.DataAnnotations;
using StarWars.Models;

namespace StarWars.ViewModels;

public class DetailsViewModel : CardViewModel
{
    [Display(Name = "Дата рождения")]
    public int BirthDate { get; set; }

    [Display(Name = "Планета")]
    [DisplayFormat(NullDisplayText = "Выберите планету...")]
    public string Planet { get; set; }

    [Display(Name = "Пол")]
    [DisplayFormat(NullDisplayText = "Выберите пол...")]
    public Gender Gender { get; set; }

    [Display(Name = "Раса")]
    [DisplayFormat(NullDisplayText = "Выберите расу...")]
    public string Race { get; set; }

    [Display(Name = "Рост")]
    [DisplayFormat(DataFormatString = "{0:N2} м", ApplyFormatInEditMode = false)]
    public decimal Height { get; set; }

    [Display(Name = "Цвет волос")]
    [DisplayFormat(NullDisplayText = "Выберите цвет...")]
    public string HairColor { get; set; }

    [Display(Name = "Цвет глаз")]
    [DisplayFormat(NullDisplayText = "Выберите цвет...")]
    public string EyeColor { get; set; }

    [Display(Name = "Описание")]
    public string History { get; set; }

    [Display(Name = "Фильмы")]
    public IEnumerable<string> Movies { get; set; }
}