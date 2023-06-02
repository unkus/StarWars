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
    public async Task<IActionResult> Index(IndexViewModel viewModel)
    {
        if (_context.Character == null)
        {
            return NotFound();
        }
        viewModel.PlanetList = await _context.Planet.Select(p => p.Name).ToListAsync();
        viewModel.MovieList = await _context.Movie.Select(m => m.Title).ToListAsync();

        if (viewModel.Gender == null)
        {
            ModelState.ClearValidationState(nameof(IndexViewModel.Gender));
            ModelState.MarkFieldSkipped(nameof(IndexViewModel.Gender));
        }

        IEnumerable<Character> characterList;
        if (ModelState.IsValid)
        {
            characterList = await _context.Character
                .Where(
                    c => (viewModel.BeginDate == null || viewModel.BeginDate < c.BirthDate)
                    && (viewModel.EndDate == null || c.BirthDate > viewModel.EndDate)
                    && (viewModel.Planet == null || c.Planet.Name.Equals(viewModel.Planet))
                    && (viewModel.Gender == null || c.Gender == viewModel.Gender)
                    && (viewModel.Movies == null || c.Movies!.Any(m => viewModel.Movies.Contains(m.Title)))
                )
                .Include(c => c.Planet)
                .Include(c => c.Movies)
                .ToListAsync();
        }
        else
        {
            characterList = await _context.Character.ToListAsync();
        }

        viewModel.Characters = characterList.Select(c => new CardViewModel
        {
            Name = c.Name,
            OriginalName = c.OriginalName
        }).ToList();

        return View(viewModel);
    }

    // GET: Details
    public async Task<IActionResult> Details(string name)
    {
        if (name == null || _context.Character == null)
        {
            return NotFound();
        }

        var character = await _context.Character
            .Include(c => c.Planet)
            .Include(c => c.Race)
            .Include(c => c.HairColor)
            .Include(c => c.EyeColor)
            .Include(c => c.Movies)
            .FirstOrDefaultAsync(c => c.Name.Equals(name));
        if (character == null)
        {
            return NotFound();
        }

        return View(new DetailsViewModel
        {
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
    public async Task<IActionResult> Create()
    {
        return View(new CreateViewModel()
        {
            PlanetList = await _context.Planet.Select(p => p.Name).ToListAsync(),
            RaceList = await _context.Race.Select(r => r.Name).ToListAsync(),
            HairColorList = await _context.HairColor.Select(h => h.Name).ToListAsync(),
            EyeColorList = await _context.EyeColor.Select(e => e.Name).ToListAsync(),
            MoviesList = await _context.Movie.Select(m => m.Title).ToListAsync()
        });
    }

    // POST: Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EditViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            Planet? planet = await _context.Planet.SingleOrDefaultAsync(p => p.Name.Equals(viewModel.Planet));
            Race? race = await _context.Race.SingleOrDefaultAsync(r => r.Name.Equals(viewModel.Race));
            HairColor? hairColor = await _context.HairColor.SingleOrDefaultAsync(h => h.Name.Equals(viewModel.HairColor));
            EyeColor? eyeColor = await _context.EyeColor.SingleOrDefaultAsync(e => e.Name.Equals(viewModel.EyeColor));
            ICollection<Movie> movies = new List<Movie>();
            foreach (string title in viewModel.Movies)
            {
                movies.Add(await _context.Movie.SingleOrDefaultAsync(e => e.Title.Equals(title)) ?? new Movie { Title = title });
            }

            _context.Add(new Character
            {
                Name = viewModel.Name,
                OriginalName = viewModel.OriginalName,
                BirthDate = viewModel.BirthDate,
                Planet = planet ?? new Planet { Name = viewModel.Planet },
                Gender = viewModel.Gender,
                Race = race ?? new Race { Name = viewModel.Race },
                Height = viewModel.Height,
                HairColor = hairColor ?? new HairColor { Name = viewModel.HairColor },
                EyeColor = eyeColor ?? new EyeColor { Name = viewModel.EyeColor },
                History = viewModel.History,
                Movies = movies
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        viewModel.PlanetList = await _context.Planet.Select(p => p.Name).ToListAsync();
        viewModel.RaceList = await _context.Race.Select(r => r.Name).ToListAsync();
        viewModel.HairColorList = await _context.HairColor.Select(h => h.Name).ToListAsync();
        viewModel.EyeColorList = await _context.EyeColor.Select(e => e.Name).ToListAsync();
        viewModel.MoviesList = await _context.Movie.Select(m => m.Title).ToListAsync();
        return View(viewModel);
    }

    // GET: Edit
    public async Task<IActionResult> Edit(string name)
    {
        if (name == null || _context.Character == null)
        {
            return NotFound();
        }

        var character = await _context.Character
            .Where(c => c.Name.Equals(name))
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

        return View(new EditViewModel
        {
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
            Movies = character.Movies.Select(m => m.Title).ToList(),

            PlanetList = await _context.Planet.Select(p => p.Name).ToListAsync(),
            RaceList = await _context.Race.Select(r => r.Name).ToListAsync(),
            HairColorList = await _context.HairColor.Select(h => h.Name).ToListAsync(),
            EyeColorList = await _context.EyeColor.Select(e => e.Name).ToListAsync(),
            MoviesList = await _context.Movie.Where(m => !character.Movies.Contains(m)).Select(m => m.Title).ToListAsync()
        });
    }

    // POST: Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? name, EditViewModel viewModel)
    {
        if (name == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                Planet? planet = await _context.Planet.SingleOrDefaultAsync(p => p.Name.Equals(viewModel.Planet));
                Race? race = await _context.Race.SingleOrDefaultAsync(r => r.Name.Equals(viewModel.Race));
                HairColor? hairColor = await _context.HairColor.SingleOrDefaultAsync(h => h.Name.Equals(viewModel.HairColor));
                EyeColor? eyeColor = await _context.EyeColor.SingleOrDefaultAsync(e => e.Name.Equals(viewModel.EyeColor));
                ICollection<Movie> movies = new List<Movie>();
                foreach (string title in viewModel.Movies)
                {
                    movies.Add(await _context.Movie.SingleOrDefaultAsync(e => e.Title.Equals(title)) ?? new Movie { Title = title });
                }

                Character? characterToUptate = await _context.Character.Where(c => c.Name.Equals(name)).Include(c => c.Movies).SingleOrDefaultAsync();
                if (characterToUptate != null)
                {
                    characterToUptate.Name = viewModel.Name;
                    characterToUptate.OriginalName = viewModel.OriginalName;
                    characterToUptate.Planet = planet ?? new Planet { Name = viewModel.Planet };
                    characterToUptate.Gender = viewModel.Gender;
                    characterToUptate.Race = race ?? new Race { Name = viewModel.Race };
                    characterToUptate.Height = viewModel.Height;
                    characterToUptate.HairColor = hairColor ?? new HairColor { Name = viewModel.HairColor };
                    characterToUptate.EyeColor = eyeColor ?? new EyeColor { Name = viewModel.EyeColor };
                    characterToUptate.Movies = movies;

                    _context.Update(characterToUptate);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CharacterExists(name))
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

        viewModel.PlanetList = await _context.Planet.Select(p => p.Name).ToListAsync();
        viewModel.RaceList = await _context.Race.Select(r => r.Name).ToListAsync();
        viewModel.HairColorList = await _context.HairColor.Select(h => h.Name).ToListAsync();
        viewModel.EyeColorList = await _context.EyeColor.Select(e => e.Name).ToListAsync();
        viewModel.MoviesList = await _context.Movie.Select(m => m.Title).ToListAsync();

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string name)
    {
        if (name == null)
        {
            return NotFound();
        }

        if (_context.Character == null)
        {
            return Problem($"Entity set ${typeof(Character)} is null.");
        }
        var character = await _context.Character.SingleOrDefaultAsync(c => c.Name.Equals(name));
        if (character != null)
        {
            _context.Character.Remove(character);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CharacterExists(string name)
    {
        return (_context.Character?.Any(e => e.Name.Equals(name))).GetValueOrDefault();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}

