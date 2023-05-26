﻿using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StarWars.Data;
using StarWars.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StarWars.ViewModels;
using System.Net;
using Microsoft.Extensions.Primitives;
using System.Linq.Expressions;

namespace StarWars.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly StarWarsContext _context;

    public HomeController(ILogger<HomeController> logger, StarWarsContext context)
    {
        _logger = logger;
        _context = context;
    }

    // GET: Index
    public async Task<IActionResult> Index(IndexViewModel indexViewModel)
    {
        indexViewModel.PlanetSelectList = new SelectList(_context.Planet, "Id", "Name", indexViewModel.PlanetID);
        indexViewModel.MovieSelectList = new MultiSelectList(_context.Movie, "Id", "Title", indexViewModel.MovieID);
      
        indexViewModel.Characters = await _context.Character
            .Where(
                c => (indexViewModel.BeginDate == null || indexViewModel.BeginDate < c.BirthDate) 
                && (indexViewModel.EndDate == null || c.BirthDate > indexViewModel.EndDate)
                && (indexViewModel.PlanetID == null || c.PlanetID == indexViewModel.PlanetID)
                && (indexViewModel.Gender == null || c.Gender == indexViewModel.Gender)
                && (indexViewModel.MovieID == null || c.Movies!.Any(m => indexViewModel.MovieID.Contains(m.Id)))
            )
            .Include(c => c.Planet)
            .Include(c => c.Movies)
            .ToListAsync();

        return View(indexViewModel);
    }

    // GET: Details
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Character == null)
        {
            return NotFound();
        }

        var character = await _context.Character
            .Include(c => c.Planet)
            .Include(c => c.Race)
            .Include(c => c.HairColor)
            .Include(c => c.EyeColor)
            .Include(c => c.Movies)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (character == null)
        {
            return NotFound();
        }

        return View(character);
    }

    // GET: Create
    public IActionResult Create()
    {
        CreateViewModel createViewModel = new CreateViewModel();
        prepareEditView(createViewModel);
        createViewModel.MoviesToSelect = new MultiSelectList(_context.Movie, "Id", "Title", createViewModel.Character?.Movies);
        return View(createViewModel);
    }

    // POST: Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateViewModel createViewModel)
    {
        await handleViewModel(createViewModel);

        if (ModelState.IsValid)
        {
            _context.Add(createViewModel.Character!);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        prepareEditView(createViewModel);
        createViewModel.MoviesToSelect = new MultiSelectList(_context.Movie, "Id", "Title", createViewModel.Character?.Movies);
        return View(createViewModel);
    }

    // GET: Edit
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Character == null)
        {
            return NotFound();
        }

        var character = await _context.Character
            .Where(c => c.Id == id)
            .Include(c => c.Planet)
            .Include(c => c.Race)
            .Include(c => c.HairColor)
            .Include(c => c.EyeColor)
            .Include(c => c.Movies)
            .SingleOrDefaultAsync();
        if (character == null)
        {
            return NotFound();
        }

        EditViewModel editViewModel = new EditViewModel { Character = character };
        prepareEditView(editViewModel);
        editViewModel.MoviesToSelect = new SelectList(_context.Movie.Where(m => !editViewModel.Character.Movies.Contains(m)), "Id", "Title");

        return View(editViewModel);
    }

    // POST: Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditViewModel editViewModel)
    {
        // editViewModel.Character.Id всегда 0
        // if (id != character.Id)
        // {
        //     return NotFound();
        // }
        if(!_context.Character.Any(c => c.Id == id))
        {
            return NotFound();
        }
        editViewModel.Character!.Id = id;

        await handleViewModel(editViewModel);
        Character? characterToUptate = await _context.Character.Where(c => c.Id == id).Include(c => c.Movies).SingleOrDefaultAsync();
        if(characterToUptate != null) {
            characterToUptate.Name = editViewModel.Character.Name;
            characterToUptate.OriginalName = editViewModel.Character.OriginalName;
            characterToUptate.PlanetID = editViewModel.Character.PlanetID;
            characterToUptate.Planet = editViewModel.Character.Planet;
            characterToUptate.RaceID = editViewModel.Character.RaceID;
            characterToUptate.Race = editViewModel.Character.Race;
            characterToUptate.Gender = editViewModel.Character.Gender;
            characterToUptate.Height = editViewModel.Character.Height;
            characterToUptate.HairColorID = editViewModel.Character.HairColorID;
            characterToUptate.HairColor = editViewModel.Character.HairColor;
            characterToUptate.EyeColorID = editViewModel.Character.EyeColorID;
            characterToUptate.EyeColor = editViewModel.Character.EyeColor;
            characterToUptate.Movies = editViewModel.Character.Movies;
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(characterToUptate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CharacterExists(characterToUptate.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        prepareEditView(editViewModel);
        editViewModel.MoviesToSelect = new SelectList(_context.Movie.Where(m => !editViewModel.Character.Movies.Contains(m)), "Id", "Title");

        return View(editViewModel);
    }

    private async Task handleViewModel(AbstractViewModel viewModel)
    {
        ModelState.ClearValidationState("Character.Planet");
        ModelState.ClearValidationState("Character.Race");
        ModelState.ClearValidationState("Character.HairColor");
        ModelState.ClearValidationState("Character.EyeColor");
        ModelState.ClearValidationState("Character.Movies");

        // TODO: откючить валидацию для PlanetID когда задано PlanetName
        // TODO: разобраться в ситуации когда заданы оба (задача валидации на клиенте)
        viewModel.Character!.Planet = await _context.Planet.SingleOrDefaultAsync(p => p.Name == viewModel.PlanetName || p.Id == viewModel.Character.PlanetID);
        if(viewModel.Character!.Planet == null && viewModel.PlanetName != null) {
            viewModel.Character!.Planet = new Planet { Name = viewModel.PlanetName };
        }
        viewModel.PlanetName = null;

        viewModel.Character!.Race = await _context.Race.SingleOrDefaultAsync(p => p.Name == viewModel.RaceName || p.Id == viewModel.Character.RaceID);
        if(viewModel.Character!.Race == null && viewModel.RaceName != null) {
            viewModel.Character!.Race = new Race { Name = viewModel.RaceName };
        }
        viewModel.RaceName = null;

        viewModel.Character!.HairColor = await _context.HairColor.SingleOrDefaultAsync(p => p.Name == viewModel.HairColorName || p.Id == viewModel.Character.HairColorID);
        if(viewModel.Character!.HairColor == null && viewModel.HairColorName != null) {
            viewModel.Character!.HairColor = new HairColor { Name = viewModel.HairColorName };
        }
        viewModel.HairColorName = null;

        viewModel.Character!.EyeColor = await _context.EyeColor.SingleOrDefaultAsync(p => p.Name == viewModel.EyeColorName || p.Id == viewModel.Character.EyeColorID);
        if(viewModel.Character!.EyeColor == null && viewModel.EyeColorName != null) {
            viewModel.Character!.EyeColor = new EyeColor { Name = viewModel.EyeColorName };
        }
        viewModel.EyeColorName = null;
        
        viewModel.Character!.Movies = viewModel.MovieID != null ? await _context.Movie.Where(m => viewModel.MovieID.Contains(m.Id)).ToListAsync() : new List<Movie>();
        IEnumerable<Movie>? moviesByTitle = viewModel.MovieTitle != null ? await _context.Movie.Where(m => viewModel.MovieTitle.Contains(m.Title)).ToListAsync() : null;
        if(viewModel.MovieTitle != null)
        {
            if(moviesByTitle != null)
            {
                foreach (var movie in moviesByTitle)
                {
                    viewModel.Character!.Movies.Add(movie);
                    viewModel.MovieTitle.Remove(movie.Title);
                }
            }
        
            foreach(var title in viewModel.MovieTitle)
            {
                if(title != null) {
                    viewModel.Character!.Movies.Add(new Movie { Title = title });
                }
            }
            viewModel.MovieTitle = null;
        }

        TryValidateModel(viewModel);
    }

    private void prepareEditView(AbstractViewModel viewModel)
    {
        viewModel.PlanetSelectList = new SelectList(_context.Planet, "Id", "Name", viewModel.Character?.PlanetID);
        viewModel.RaceSelectList = new SelectList(_context.Race, "Id", "Name", viewModel.Character?.RaceID);
        viewModel.HairColorSelectList = new SelectList(_context.HairColor, "Id", "Name", viewModel.Character?.HairColorID);
        viewModel.EyeColorSelectList = new SelectList(_context.EyeColor, "Id", "Name", viewModel.Character?.EyeColorID);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Character == null)
        {
            return Problem($"Entity set ${typeof(Character)} is null.");
        }
        var character = await _context.Character.FindAsync(id);
        if (character != null)
        {
            _context.Character.Remove(character);
        }
    
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CharacterExists(int id)
    {
      return (_context.Character?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}

