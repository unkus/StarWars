using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StarWars.Models;
using StarWars.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StarWars.ViewModels;

public class CreateViewModel
{

    public string Name { get; set; }

    [Display(Name = "Имя (в оригинале)")]
    public string OriginalName { get; set; }

    [Display(Name = "Дата рождения")]
    public int BirthDate { get; set; }

    [Display(Name = "Планета")]
    public int PlanetID { get; set; }

    [Display(Name = "Пол")]
    public Gender Gender { get; set; }

    [Display(Name = "Раса")]
    public int RaceID { get; set; }

    [Display(Name = "Рост")]
    [DisplayFormat(DataFormatString = "{0:N2} м", ApplyFormatInEditMode = false)]
    public decimal Height { get; set; }

    [Display(Name = "Цвет волос")]
    public int HairColorID { get; set; }

    [Display(Name = "Цвет глаз")]
    public int EyeColorID { get; set; }

    [Display(Name = "Описание")]
    public string History { get; set; }

    public string? PlanetName { get; set; }

    public string? RaceName { get; set; }

    public string? HairColorName { get; set; }

    public string? EyeColorName { get; set; }

    [Display(Name = "Фильмы")]
    public IEnumerable<int>? MovieID { get; set; }

    [Display(Name = "Новый фильм")]
    public ICollection<string>? MovieTitle { get; set; }

    [BindNever]
    public IEnumerable<SelectListItem>? PlanetSelectList { get; set; }

    [BindNever]
    public IEnumerable<SelectListItem>? RaceSelectList { get; set; }

    [BindNever]
    public IEnumerable<SelectListItem>? HairColorSelectList { get; set; }

    [BindNever]
    public IEnumerable<SelectListItem>? EyeColorSelectList { get; set; }

    [BindNever]
    public IEnumerable<SelectListItem>? MoviesSelectList { get; set; }
}