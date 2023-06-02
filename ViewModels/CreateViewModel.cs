using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StarWars.ViewModels;

public class CreateViewModel : DetailsViewModel
{

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