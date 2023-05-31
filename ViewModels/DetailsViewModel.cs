using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StarWars.Models;
using StarWars.Data;

namespace StarWars.ViewModels;

public class DetailsViewModel
{

    public int Id { get; set; }

    [Display(Name = "Имя персонажа")]
    public string Name { get; set; }

    [Display(Name = "Имя (в оригинале)")]
    public string OriginalName { get; set; }

    [Display(Name = "Дата рождения")]
    public int BirthDate { get; set; }

    [Display(Name = "Планета")]
    public string Planet { get; set; }

    [Display(Name = "Пол")]
    public Gender Gender { get; set; }

    [Display(Name = "Раса")]
    public string Race { get; set; }

    [Display(Name = "Рост")]
    [DisplayFormat(DataFormatString = "{0:N2} м", ApplyFormatInEditMode = false)]
    public decimal Height { get; set; }

    [Display(Name = "Цвет волос")]
    public string HairColor { get; set; }

    [Display(Name = "Цвет глаз")]
    public string EyeColor { get; set; }

    [Display(Name = "Описание")]
    public string History { get; set; }

    [Display(Name = "Фильмы")]
    public virtual IEnumerable<String> Movies { get; set; }
}