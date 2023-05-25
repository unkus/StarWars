using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StarWars.Models;

namespace StarWars.ViewModels;

public class IndexViewModel
{
    public Range? BirthDate { get; set; }
    
    // думаю это плохое решение для текста
    [Display(Name = "Дата рождения c")]
    public int? BeginDate { get; set; }
    
    // тоже что и для BeginDate
    [Display(Name = "по")]
    public int? EndDate { get; set; }

    [Display(Name = "Планета")]
    [DisplayFormat(NullDisplayText = "Выберите планету...")]
    public int? PlanetID { get; set; }
    
    [Display(Name = "Пол")]
    [DisplayFormat(NullDisplayText = "Выберите пол...")]
    public Gender? Gender { get; set; }

    [Display(Name = "Фильмы")]
    [DisplayFormat(NullDisplayText = "Выберите фильмы...")]
    public IEnumerable<int>? MovieID { get; set; }

    public ICollection<Character>? Characters { get; set; }

    public IEnumerable<SelectListItem>? PlanetSelectList { get; set; }
    public IEnumerable<SelectListItem>? MovieSelectList { get; set; }
}