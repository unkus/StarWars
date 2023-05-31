using System.Diagnostics;
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
        indexViewModel.PlanetSelectList = new SelectList(_context.Planet, nameof(Planet.Id), nameof(Planet.Name), indexViewModel.PlanetID);
        indexViewModel.MovieSelectList = new MultiSelectList(_context.Movie, nameof(Movie.Id), nameof(Movie.Title), indexViewModel.MovieID);

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

        return View(new DetailsViewModel
        {
            Id = character.Id,
            Name = character.Name,
            OriginalName = character.OriginalName,
            BirthDate = character.BirthDate,
            Planet = character.Planet.Name,
            Gender = character.Gender,
            Race = character.Race.Name,
            Height = character.Height,
            HairColor = character.HairColor.Name,
            EyeColor = character.EyeColor.Name,
            History = character.History,
            Movies = character.Movies.Select(m => m.Title)
        });
    }

    // GET: Create
    public IActionResult Create()
    {
        CreateViewModel createViewModel = new CreateViewModel()
        {
            PlanetSelectList = new SelectList(_context.Planet, nameof(Planet.Id), nameof(Planet.Name)),
            RaceSelectList = new SelectList(_context.Race, nameof(Race.Id), nameof(Race.Name)),
            HairColorSelectList = new SelectList(_context.HairColor, nameof(HairColor.Id), nameof(HairColor.Name)),
            EyeColorSelectList = new SelectList(_context.EyeColor, nameof(EyeColor.Id), nameof(EyeColor.Name)),
            MoviesSelectList = new MultiSelectList(_context.Movie, nameof(Movie.Id), nameof(Movie.Title))
        };
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
            _context.Add(new Character
            {
                Name = createViewModel.Name,
                OriginalName = createViewModel.OriginalName,
                BirthDate = createViewModel.BirthDate,
                PlanetID = createViewModel.PlanetID,
                Gender = createViewModel.Gender,
                RaceID = createViewModel.RaceID,
                HairColorID = createViewModel.HairColorID,
                EyeColorID = createViewModel.EyeColorID,
                History = createViewModel.History,
                Movies = _context.Movie.Where(m => createViewModel.MovieID.Contains(m.Id)).ToList()
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        createViewModel.PlanetSelectList = new SelectList(_context.Planet, nameof(Planet.Id), nameof(Planet.Name), createViewModel.PlanetID);
        createViewModel.RaceSelectList = new SelectList(_context.Race, nameof(Race.Id), nameof(Race.Name), createViewModel.RaceID);
        createViewModel.HairColorSelectList = new SelectList(_context.HairColor, nameof(HairColor.Id), nameof(HairColor.Name), createViewModel.HairColorID);
        createViewModel.EyeColorSelectList = new SelectList(_context.EyeColor, nameof(EyeColor.Id), nameof(EyeColor.Name), createViewModel.EyeColorID);
        createViewModel.MoviesSelectList = new MultiSelectList(_context.Movie, nameof(Movie.Id), nameof(Movie.Title), createViewModel.MovieID);
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

        EditViewModel editViewModel = new EditViewModel
        {
            Id = character.Id,
            Name = character.Name,
            OriginalName = character.OriginalName,
            BirthDate = character.BirthDate,
            PlanetID = character.PlanetID,
            Gender = character.Gender,
            RaceID = character.RaceID,
            Height = character.Height,
            HairColorID = character.HairColorID,
            EyeColorID = character.EyeColorID,
            History = character.History,
            PlanetSelectList = new SelectList(_context.Planet, nameof(Planet.Id), nameof(Planet.Name), character.PlanetID),
            RaceSelectList = new SelectList(_context.Race, nameof(Race.Id), nameof(Race.Name), character.RaceID),
            HairColorSelectList = new SelectList(_context.HairColor, nameof(HairColor.Id), nameof(HairColor.Name), character.HairColorID),
            EyeColorSelectList = new SelectList(_context.EyeColor, nameof(EyeColor.Id), nameof(EyeColor.Name), character.EyeColorID),
            MoviesSelectList = new SelectList(_context.Movie.Where(m => !character.Movies.Contains(m)), nameof(Movie.Id), nameof(Movie.Title))
        };

        return View(editViewModel);
    }

    // POST: Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditViewModel editViewModel)
    {
        if (id != editViewModel.Id)
        {
            return NotFound();
        }

        await handleViewModel(editViewModel);
        if (ModelState.IsValid)
        {
            try
            {
                Character? characterToUptate = await _context.Character.Where(c => c.Id == id).Include(c => c.Movies).SingleOrDefaultAsync();
                if (characterToUptate != null)
                {
                    characterToUptate.Name = editViewModel.Name;
                    characterToUptate.OriginalName = editViewModel.OriginalName;
                    characterToUptate.PlanetID = editViewModel.PlanetID;
                    characterToUptate.RaceID = editViewModel.RaceID;
                    characterToUptate.Gender = editViewModel.Gender;
                    characterToUptate.Height = editViewModel.Height;
                    characterToUptate.HairColorID = editViewModel.HairColorID;
                    characterToUptate.EyeColorID = editViewModel.EyeColorID;
                    characterToUptate.Movies = _context.Movie.Where(m => editViewModel.MovieID.Contains(m.Id)).ToList();
                }

                _context.Update(characterToUptate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CharacterExists(id))
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

        editViewModel.PlanetSelectList = new SelectList(_context.Planet, nameof(Planet.Id), nameof(Planet.Name), editViewModel.PlanetID);
        editViewModel.RaceSelectList = new SelectList(_context.Race, nameof(Race.Id), nameof(Race.Name), editViewModel.RaceID);
        editViewModel.HairColorSelectList = new SelectList(_context.HairColor, nameof(HairColor.Id), nameof(HairColor.Name), editViewModel.HairColorID);
        editViewModel.EyeColorSelectList = new SelectList(_context.EyeColor, nameof(EyeColor.Id), nameof(EyeColor.Name), editViewModel.EyeColorID);
        editViewModel.MoviesSelectList = new SelectList(_context.Movie.Where(m => !editViewModel.MovieID.Contains(m.Id)), nameof(Movie.Id), nameof(Movie.Title));

        return View(editViewModel);
    }

    private async Task handleViewModel(CreateViewModel viewModel)
    {
        // TODO: откючить валидацию для PlanetID когда задано PlanetName
        // TODO: разобраться в ситуации когда заданы оба (задача валидации на клиенте)
        // можно будет убрать проверку на null для viewModel.PlanetName
        var planet = await _context.Planet.SingleOrDefaultAsync(p => p.Name.Equals(viewModel.PlanetName)
                                                                                        || (viewModel.PlanetName == null && p.Id == viewModel.PlanetID));
        if (planet == null && viewModel.PlanetName != null)
        {
            planet = new Planet { Name = viewModel.PlanetName };
        }
        viewModel.PlanetName = null;

        var race = await _context.Race.SingleOrDefaultAsync(r => r.Name.Equals(viewModel.RaceName)
                                                                                        || (viewModel.RaceName == null && r.Id == viewModel.RaceID));
        if (race == null && viewModel.RaceName != null)
        {
            race = new Race { Name = viewModel.RaceName };
        }
        viewModel.RaceName = null;

        var hairColor = await _context.HairColor.SingleOrDefaultAsync(h => h.Name.Equals(viewModel.HairColorName)
                                                                                        || (viewModel.HairColorName == null && h.Id == viewModel.HairColorID));
        if (hairColor == null && viewModel.HairColorName != null)
        {
            hairColor = new HairColor { Name = viewModel.HairColorName };
        }
        viewModel.HairColorName = null;

        var eyeColor = await _context.EyeColor.SingleOrDefaultAsync(e => e.Name.Equals(viewModel.EyeColorName)
                                                                                        || (viewModel.EyeColorName == null && e.Id == viewModel.EyeColorID));
        if (eyeColor == null && viewModel.EyeColorName != null)
        {
            eyeColor = new EyeColor { Name = viewModel.EyeColorName };
        }
        viewModel.EyeColorName = null;

        var movies = viewModel.MovieID != null ? await _context.Movie.Where(m => viewModel.MovieID.Contains(m.Id)).ToListAsync() : new List<Movie>();
        IEnumerable<Movie>? moviesByTitle = viewModel.MovieTitle != null ? await _context.Movie.Where(m => viewModel.MovieTitle.Contains(m.Title)).ToListAsync() : null;
        if (viewModel.MovieTitle != null)
        {
            if (moviesByTitle != null)
            {
                foreach (var movie in moviesByTitle)
                {
                    movies.Add(movie);
                    viewModel.MovieTitle.Remove(movie.Title);
                }
            }

            foreach (var title in viewModel.MovieTitle)
            {
                if (title != null)
                {
                    movies.Add(new Movie { Title = title });
                }
            }
            viewModel.MovieTitle = null;
        }

        TryValidateModel(viewModel);
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

