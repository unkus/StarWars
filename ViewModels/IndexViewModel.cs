using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StarWars.Models;

namespace StarWars.ViewModels;

public class IndexViewModel
{
    // думаю это плохое решение для текста
    [Display(Name = "Дата рождения c")]
    public int? BeginDate { get; set; }

    // тоже что и для BeginDate
    [Display(Name = "по")]
    public int? EndDate { get; set; }

    [Display(Name = "Планета")]
    [DisplayFormat(NullDisplayText = "Выберите планету...")]
    public string? Planet { get; set; }

    [Display(Name = "Пол")]
    [DisplayFormat(NullDisplayText = "Выберите пол...")]
    public Gender? Gender { get; set; }

    [Display(Name = "Фильмы")]
    [DisplayFormat(NullDisplayText = "Выберите фильмы...")]
    public IEnumerable<string>? Movies { get; set; }

    public ICollection<CardViewModel>? Characters { get; set; }

    public IEnumerable<string>? PlanetList { get; set; }
    public IEnumerable<string>? MovieList { get; set; }
}