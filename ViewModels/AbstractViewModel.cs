using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StarWars.Models;

namespace StarWars.ViewModels;

public abstract class AbstractViewModel
{

    public Character? Character { get; set; }

    public string? PlanetName { get; set; }

    public IEnumerable<SelectListItem>? PlanetsToSelect { get; set; }
    
    public string? RaceName { get; set; }

    public IEnumerable<SelectListItem>? RaceToSelect { get; set; }

    public string? HairColorName { get; set; }

    public string? EyeColorName { get; set; }

    [Display(Name = "Фильмы")]
    public IEnumerable<int>? MovieID { get; set; }
   
    [Display(Name = "Новый фильм")]
    public ICollection<string>? MovieTitle { get; set; }

    public IEnumerable<SelectListItem>? PlanetSelectList { get; set; }
    public IEnumerable<SelectListItem>? RaceSelectList { get; set; }
    public IEnumerable<SelectListItem>? HairColorSelectList { get; set; }
    public IEnumerable<SelectListItem>? EyeColorSelectList { get; set; }
    public IEnumerable<SelectListItem>? MoviesToSelect { get; set; }
}