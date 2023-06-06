using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using StarWars.Data;
using StarWars.Models;
using StarWars.ViewModels;
using AutoMapper;

namespace StarWars.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMapper _mapping;

    private UnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, StarWarsContext context, IMapper mapping)
    {
        _logger = logger;
        _mapping = mapping;
        _unitOfWork = new UnitOfWork { _context = context };
    }

    // GET: Index
    public IActionResult Index(IndexViewModel viewModel)
    {
        viewModel.PlanetList = _unitOfWork.PlanetRepository.Get().Select(p => p.Name).ToList();
        viewModel.MovieList = _unitOfWork.MovieRepository.Get().Select(m => m.Title).ToList();

        if (viewModel.Gender is null)
        {
            ModelState.ClearValidationState(nameof(IndexViewModel.Gender));
            ModelState.MarkFieldSkipped(nameof(IndexViewModel.Gender));
        }

        IEnumerable<Character>? characterList;
        if (ModelState.IsValid)
        {
            characterList = _unitOfWork.CharacterRepository.Get(
                    filter: c => (viewModel.BeginDate == null || viewModel.BeginDate < c.BirthDate)
                    && (viewModel.EndDate == null || c.BirthDate > viewModel.EndDate)
                    && (viewModel.Planet == null || c.Planet.Name.Equals(viewModel.Planet))
                    && (viewModel.Gender == null || c.Gender == viewModel.Gender)
                    && (viewModel.Movies == null || c.Movies!.Any(m => viewModel.Movies.Contains(m.Title))));
        }
        else
        {
            characterList = _unitOfWork.CharacterRepository.Get();
        }

        if (characterList is not null)
        {
            viewModel.Characters = characterList.Select(c => _mapping.Map<CardViewModel>(c)).ToList();
        }

        return View(viewModel);
    }

    // GET: Details
    public IActionResult Details(string name)
    {
        if (name is null)
        {
            return NotFound();
        }
        var character = _unitOfWork.CharacterRepository.SingleOrDefault(
            filter: c => c.Name.Equals(name),
            includes: new List<string> 
                {
                    nameof(Character.Planet),
                    nameof(Character.Race),
                    nameof(Character.HairColor),
                    nameof(Character.EyeColor),
                    nameof(Character.Movies)
                }
            );
        if (character is null)
        {
            return NotFound();
        }

        return View(_mapping.Map<DetailsViewModel>(character));
    }

    // GET: Create
    public IActionResult Create()
    {
        return View(new CreateViewModel()
        {
            PlanetList = _unitOfWork.PlanetRepository.Get().Select(p => p.Name).ToList(),
            RaceList = _unitOfWork.RaceRepository.Get().Select(r => r.Name).ToList(),
            HairColorList = _unitOfWork.HairColorRepository.Get().Select(h => h.Name).ToList(),
            EyeColorList = _unitOfWork.EyeColorRepository.Get().Select(e => e.Name).ToList(),
            MoviesList = _unitOfWork.MovieRepository.Get().Select(m => m.Title).ToList()
        });
    }

    // POST: Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var planet = _unitOfWork.PlanetRepository.SingleOrDefault(p => p.Name.Equals(viewModel.Planet));
            var race = _unitOfWork.RaceRepository.SingleOrDefault(r => r.Name.Equals(viewModel.Race));
            var hairColor = _unitOfWork.HairColorRepository.SingleOrDefault(h => h.Name.Equals(viewModel.HairColor));
            var eyeColor = _unitOfWork.EyeColorRepository.SingleOrDefault(e => e.Name.Equals(viewModel.EyeColor));
            var movies = new List<Movie>();
            foreach (string title in viewModel.Movies)
            {
                movies.Add(_unitOfWork.MovieRepository.SingleOrDefault(e => e.Title.Equals(title)) ?? new Movie { Title = title });
            }

            var character = _mapping.Map<Character>(viewModel);
            character.Planet = planet ?? character.Planet;
            character.Race = race ?? character.Race;
            character.HairColor = hairColor ?? character.HairColor;
            character.EyeColor = eyeColor ?? character.EyeColor;
            character.Movies = movies;

            _unitOfWork.CharacterRepository.Insert(character);

            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        viewModel.PlanetList = _unitOfWork.PlanetRepository.Get().Select(p => p.Name).ToList();
        viewModel.RaceList = _unitOfWork.RaceRepository.Get().Select(r => r.Name).ToList();
        viewModel.HairColorList = _unitOfWork.HairColorRepository.Get().Select(h => h.Name).ToList();
        viewModel.EyeColorList = _unitOfWork.EyeColorRepository.Get().Select(e => e.Name).ToList();
        viewModel.MoviesList = _unitOfWork.MovieRepository.Get().Select(m => m.Title).ToList();
        return View(viewModel);
    }

    // GET: Edit
    public IActionResult Edit(string name)
    {
        if (name is null)
        {
            return NotFound();
        }

        var character = _unitOfWork.CharacterRepository.SingleOrDefault(c => c.Name.Equals(name));
        if (character is null)
        {
            return NotFound();
        }

        var viewModel = _mapping.Map<EditViewModel>(character);
        viewModel.PlanetList = _unitOfWork.PlanetRepository.Get().Select(p => p.Name).ToList();
        viewModel.RaceList = _unitOfWork.RaceRepository.Get().Select(r => r.Name).ToList();
        viewModel.HairColorList = _unitOfWork.HairColorRepository.Get().Select(h => h.Name).ToList();
        viewModel.EyeColorList = _unitOfWork.EyeColorRepository.Get().Select(e => e.Name).ToList();
        viewModel.MoviesList = _unitOfWork.MovieRepository.Get(m => !character.Movies.Contains(m)).Select(m => m.Title).ToList();

        return View(viewModel);
    }

    // POST: Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? name, EditViewModel viewModel)
    {
        if (name is null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var characterToUptate = _unitOfWork.CharacterRepository.SingleOrDefault(c => c.Name.Equals(name));
                if (characterToUptate is null)
                {
                    return NotFound();
                }

                var planet = _unitOfWork.PlanetRepository.SingleOrDefault(p => p.Name.Equals(viewModel.Planet));
                var race = _unitOfWork.RaceRepository.SingleOrDefault(r => r.Name.Equals(viewModel.Race));
                var hairColor = _unitOfWork.HairColorRepository.SingleOrDefault(h => h.Name.Equals(viewModel.HairColor));
                var eyeColor = _unitOfWork.EyeColorRepository.SingleOrDefault(e => e.Name.Equals(viewModel.EyeColor));
                var movies = new List<Movie>();
                foreach (string title in viewModel.Movies)
                {
                    movies.Add(_unitOfWork.MovieRepository.SingleOrDefault(e => e.Title.Equals(title)) ?? new Movie { Title = title });
                }

                var character = _mapping.Map<EditViewModel, Character>(viewModel, characterToUptate);
                character.Planet = planet ?? character.Planet;
                character.Race = race ?? character.Race;
                character.HairColor = hairColor ?? character.HairColor;
                character.EyeColor = eyeColor ?? character.EyeColor;
                character.Movies = movies;

                _unitOfWork.CharacterRepository.Update(characterToUptate);
                await _unitOfWork.Save();
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

        viewModel.PlanetList = _unitOfWork.PlanetRepository.Get().Select(p => p.Name).ToList();
        viewModel.RaceList = _unitOfWork.RaceRepository.Get().Select(r => r.Name).ToList();
        viewModel.HairColorList = _unitOfWork.HairColorRepository.Get().Select(h => h.Name).ToList();
        viewModel.EyeColorList = _unitOfWork.EyeColorRepository.Get().Select(e => e.Name).ToList();
        viewModel.MoviesList = _unitOfWork.MovieRepository.Get().Select(m => m.Title).ToList();

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string name)
    {
        if (name is null)
        {
            return NotFound();
        }

        var character = _unitOfWork.CharacterRepository.SingleOrDefault(c => c.Name.Equals(name));
        if (character is not null)
        {
            _unitOfWork.CharacterRepository.Delete(character);
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    private bool CharacterExists(string name)
    {
        return _unitOfWork.CharacterRepository.SingleOrDefault(c => c.Name.Equals(name)) is not null;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}

