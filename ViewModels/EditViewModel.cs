using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StarWars.Models;
using StarWars.Data;

namespace StarWars.ViewModels;

public class EditViewModel: CreateViewModel
{
    public int Id { get; set; }
}