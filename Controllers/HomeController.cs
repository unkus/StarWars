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
        indexViewModel.PlanetList = await _context.Planet.Select(p => p.Name).ToListAsync();
        indexViewModel.MovieList = await _context.Movie.Select(m => m.Title).ToListAsync();

        IEnumerable<Character> characterList = await _context.Character
            .Where(
                c => (indexViewModel.BeginDate == null || indexViewModel.BeginDate < c.BirthDate)
                && (indexViewModel.EndDate == null || c.BirthDate > indexViewModel.EndDate)
                && (indexViewModel.Planet == null || c.Planet.Equals(indexViewModel.Planet))
                && (indexViewModel.Gender == null || c.Gender == indexViewModel.Gender)
                && (indexViewModel.Movies == null || c.Movies!.Any(m => indexViewModel.Movies.Contains(m.Title)))
            )
            .Include(c => c.Planet)
            .Include(c => c.Movies)
            .ToListAsync();

        indexViewModel.Characters = characterList.Select(c => new CardViewModel()
        {
            Name = c.Name,
            OriginalName = c.OriginalName
        }).ToList();

        return View(indexViewModel);
    }

    // GET: Details
    public async Task<IActionResult> Details(string id)
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
            .FirstOrDefaultAsync(c => c.Name.Equals(id));
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
        CreateViewModel createViewModel = new CreateViewModel()
        {
            PlanetList = await _context.Planet.Select(p => p.Name).ToListAsync(),
            RaceList = await _context.Race.Select(r => r.Name).ToListAsync(),
            HairColorList = await _context.HairColor.Select(h => h.Name).ToListAsync(),
            EyeColorList = await _context.EyeColor.Select(e => e.Name).ToListAsync(),
            MoviesList = await _context.Movie.Select(m => m.Title).ToListAsync()
        };
        return View(createViewModel);
    }

    // POST: Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EditViewModel viewModel)
    {
        await handleViewModel(viewModel);

        if (ModelState.IsValid)
        {
            _context.Add(new Character
            {
                Name = viewModel.Name,
                OriginalName = viewModel.OriginalName,
                BirthDate = viewModel.BirthDate,
                PlanetID = _context.Planet.Where(p => p.Name.Equals(viewModel.Planet)).Select(p => p.Id).SingleOrDefault(),
                Gender = viewModel.Gender,
                RaceID = _context.Race.Where(r => r.Name.Equals(viewModel.Race)).Select(r => r.Id).SingleOrDefault(),
                HairColorID = _context.HairColor.Where(h => h.Name.Equals(viewModel.HairColor)).Select(h => h.Id).SingleOrDefault(),
                EyeColorID = _context.EyeColor.Where(e => e.Name.Equals(viewModel.EyeColor)).Select(e => e.Id).SingleOrDefault(),
                History = viewModel.History,
                Movies = _context.Movie.Where(m => viewModel.Movies.Contains(m.Title)).ToList()
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
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null || _context.Character == null)
        {
            return NotFound();
        }

        var character = await _context.Character
            .Where(c => c.Name.Equals(id))
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
        };

        return View(editViewModel);
    }

    // POST: Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string name, EditViewModel editViewModel)
    {
        if (!name.Equals(editViewModel.Name))
        {
            return NotFound();
        }

        await handleViewModel(editViewModel);
        if (ModelState.IsValid)
        {
            try
            {
                Character? characterToUptate = await _context.Character.Where(c => c.Name.Equals(name)).Include(c => c.Movies).SingleOrDefaultAsync();
                if (characterToUptate != null)
                {
                    characterToUptate.Name = editViewModel.Name;
                    characterToUptate.OriginalName = editViewModel.OriginalName;
                    characterToUptate.PlanetID = _context.Planet.Where(p => p.Name.Equals(editViewModel.Planet)).Select(p => p.Id).SingleOrDefault();
                    characterToUptate.RaceID = _context.Race.Where(r => r.Name.Equals(editViewModel.Race)).Select(r => r.Id).SingleOrDefault();
                    characterToUptate.Gender = editViewModel.Gender;
                    characterToUptate.Height = editViewModel.Height;
                    characterToUptate.HairColorID = _context.HairColor.Where(h => h.Name.Equals(editViewModel.HairColor)).Select(h => h.Id).SingleOrDefault();
                    characterToUptate.EyeColorID = _context.EyeColor.Where(e => e.Name.Equals(editViewModel.EyeColor)).Select(e => e.Id).SingleOrDefault();
                    characterToUptate.Movies = _context.Movie.Where(m => editViewModel.Movies.Contains(m.Title)).ToList();
                }

                _context.Update(characterToUptate);
                await _context.SaveChangesAsync();
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

        editViewModel.PlanetList = await _context.Planet.Select(p => p.Name).ToListAsync();
        editViewModel.RaceList = await _context.Race.Select(r => r.Name).ToListAsync();
        editViewModel.HairColorList = await _context.HairColor.Select(h => h.Name).ToListAsync();
        editViewModel.EyeColorList = await _context.EyeColor.Select(e => e.Name).ToListAsync();
        editViewModel.MoviesList = await _context.Movie.Select(m => m.Title).ToListAsync();

        return View(editViewModel);
    }

    private async Task handleViewModel(CreateViewModel viewModel)
    {
        // TODO: откючить валидацию для PlanetID когда задано PlanetName
        // TODO: разобраться в ситуации когда заданы оба (задача валидации на клиенте)
        // можно будет убрать проверку на null для viewModel.PlanetName
        var planet = await _context.Planet.SingleOrDefaultAsync(p => p.Name.Equals(viewModel.Planet));
        if (planet == null)
        {
            planet = new Planet { Name = viewModel.Planet };
        }

        var race = await _context.Race.SingleOrDefaultAsync(r => r.Name.Equals(viewModel.Race));
        if (race == null)
        {
            race = new Race { Name = viewModel.Race };
        }

        var hairColor = await _context.HairColor.SingleOrDefaultAsync(h => h.Name.Equals(viewModel.HairColor));
        if (hairColor == null)
        {
            hairColor = new HairColor { Name = viewModel.HairColor };
        }

        var eyeColor = await _context.EyeColor.SingleOrDefaultAsync(e => e.Name.Equals(viewModel.EyeColor));
        if (eyeColor == null)
        {
            eyeColor = new EyeColor { Name = viewModel.EyeColor };
        }

        var movies = viewModel.Movies != null ? await _context.Movie.Where(m => viewModel.Movies.Contains(m.Title)).ToListAsync() : new List<Movie>();
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
    public async Task<IActionResult> DeleteConfirmed(string name)
    {
        if (_context.Character == null)
        {
            return Problem($"Entity set ${typeof(Character)} is null.");
        }
        var character = await _context.Character.FindAsync(name);
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

