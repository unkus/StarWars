using System.ComponentModel.DataAnnotations;

namespace StarWars.ViewModels;

public class CardViewModel
{
    [Display(Name = "Имя персонажа")]
    public string Name { get; set; }

    [Display(Name = "Имя (в оригинале)")]
    public string OriginalName { get; set; }

}