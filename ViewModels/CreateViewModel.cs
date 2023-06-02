using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StarWars.ViewModels;

public class CreateViewModel : DetailsViewModel
{

    public string? PlanetName { get; set; }

    public string? RaceName { get; set; }

    public string? HairColorName { get; set; }

    public string? EyeColorName { get; set; }

    [Display(Name = "Новый фильм")]
    public ICollection<string>? MovieTitle { get; set; }

    [BindNever]
    public IEnumerable<string>? PlanetList { get; set; }

    [BindNever]
    public IEnumerable<string>? RaceList { get; set; }

    [BindNever]
    public IEnumerable<string>? HairColorList { get; set; }

    [BindNever]
    public IEnumerable<string>? EyeColorList { get; set; }

    [BindNever]
    public IEnumerable<string>? MoviesList { get; set; }
}