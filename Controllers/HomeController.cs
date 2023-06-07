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

    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IMapper mapping)
    {
        _logger = logger;
        _mapping = mapping;
        _unitOfWork = unitOfWork;
    }

    // GET: Index
    public async Task<IActionResult> Index(IndexViewModel viewModel)
    {
        viewModel.PlanetList = (await _unitOfWork.PlanetRepository.GetAsync()).Select(p => p.Name).ToList();
        viewModel.MovieList = (await _unitOfWork.MovieRepository.GetAsync()).Select(m => m.Title).ToList();

        if (viewModel.Gender is null)
        {
            ModelState.ClearValidationState(nameof(IndexViewModel.Gender));
            ModelState.MarkFieldSkipped(nameof(IndexViewModel.Gender));
        }

        IEnumerable<Character>? characterList;
        if (ModelState.IsValid)
        {
            characterList = await _unitOfWork.CharacterRepository.GetAsync(
                    filter: c => (viewModel.BeginDate == null || viewModel.BeginDate < c.BirthDate)
                    && (viewModel.EndDate == null || c.BirthDate > viewModel.EndDate)
                    && (viewModel.Planet == null || c.Planet.Name.Equals(viewModel.Planet))
                    && (viewModel.Gender == null || c.Gender == viewModel.Gender)
                    && (viewModel.Movies == null || c.Movies!.Any(m => viewModel.Movies.Contains(m.Title))));
        }
        else
        {
            characterList = await _unitOfWork.CharacterRepository.GetAsync();
        }

        if (characterList is not null)
        {
            viewModel.Characters = characterList.Select(c => _mapping.Map<CardViewModel>(c)).ToList();
        }

        return View(viewModel);
    }

    // GET: Details
    public async Task<IActionResult> Details(string name)
    {
        if (name is null)
        {
            return NotFound();
        }
        var character = await _unitOfWork.CharacterRepository.SingleOrDefaultAsync(
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
    public async Task<IActionResult> Create()
    {
        return View(new CreateViewModel()
        {
            PlanetList = (await _unitOfWork.PlanetRepository.GetAsync()).Select(p => p.Name).ToList(),
            RaceList = (await _unitOfWork.RaceRepository.GetAsync()).Select(r => r.Name).ToList(),
            HairColorList = (await _unitOfWork.HairColorRepository.GetAsync()).Select(h => h.Name).ToList(),
            EyeColorList = (await _unitOfWork.EyeColorRepository.GetAsync()).Select(e => e.Name).ToList(),
            MoviesList = (await _unitOfWork.MovieRepository.GetAsync()).Select(m => m.Title).ToList()
        });
    }

    // POST: Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var planet = await _unitOfWork.PlanetRepository.SingleOrDefaultAsync(p => p.Name.Equals(viewModel.Planet));
            var race = await _unitOfWork.RaceRepository.SingleOrDefaultAsync(r => r.Name.Equals(viewModel.Race));
            var hairColor = await _unitOfWork.HairColorRepository.SingleOrDefaultAsync(h => h.Name.Equals(viewModel.HairColor));
            var eyeColor = await _unitOfWork.EyeColorRepository.SingleOrDefaultAsync(e => e.Name.Equals(viewModel.EyeColor));
            var movies = new List<Movie>();
            foreach (string title in viewModel.Movies)
            {
                movies.Add(await _unitOfWork.MovieRepository.SingleOrDefaultAsync(e => e.Title.Equals(title)) ?? new Movie { Title = title });
            }

            var character = _mapping.Map<Character>(viewModel);
            character.Planet = planet ?? character.Planet;
            character.Race = race ?? character.Race;
            character.HairColor = hairColor ?? character.HairColor;
            character.EyeColor = eyeColor ?? character.EyeColor;
            character.Movies = movies;

            _unitOfWork.CharacterRepository.Insert(character);

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        viewModel.PlanetList = (await _unitOfWork.PlanetRepository.GetAsync()).Select(p => p.Name).ToList();
        viewModel.RaceList = (await _unitOfWork.RaceRepository.GetAsync()).Select(r => r.Name).ToList();
        viewModel.HairColorList = (await _unitOfWork.HairColorRepository.GetAsync()).Select(h => h.Name).ToList();
        viewModel.EyeColorList = (await _unitOfWork.EyeColorRepository.GetAsync()).Select(e => e.Name).ToList();
        viewModel.MoviesList = (await _unitOfWork.MovieRepository.GetAsync()).Select(m => m.Title).ToList();
        return View(viewModel);
    }

    // GET: Edit
    public async Task<IActionResult> Edit(string name)
    {
        if (name is null)
        {
            return NotFound();
        }

        var character = await _unitOfWork.CharacterRepository.SingleOrDefaultAsync(
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

        var viewModel = _mapping.Map<EditViewModel>(character);
        viewModel.PlanetList = (await _unitOfWork.PlanetRepository.GetAsync()).Select(p => p.Name).ToList();
        viewModel.RaceList = (await _unitOfWork.RaceRepository.GetAsync()).Select(r => r.Name).ToList();
        viewModel.HairColorList = (await _unitOfWork.HairColorRepository.GetAsync()).Select(h => h.Name).ToList();
        viewModel.EyeColorList = (await _unitOfWork.EyeColorRepository.GetAsync()).Select(e => e.Name).ToList();
        viewModel.MoviesList = (await _unitOfWork.MovieRepository.GetAsync(m => !character.Movies.Contains(m))).Select(m => m.Title).ToList();

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
            var characterToUptate = await _unitOfWork.CharacterRepository.SingleOrDefaultAsync(
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
            if (characterToUptate is null)
            {
                return NotFound();
            }

            var planet = await _unitOfWork.PlanetRepository.SingleOrDefaultAsync(p => p.Name.Equals(viewModel.Planet));
            var race = await _unitOfWork.RaceRepository.SingleOrDefaultAsync(r => r.Name.Equals(viewModel.Race));
            var hairColor = await _unitOfWork.HairColorRepository.SingleOrDefaultAsync(h => h.Name.Equals(viewModel.HairColor));
            var eyeColor = await _unitOfWork.EyeColorRepository.SingleOrDefaultAsync(e => e.Name.Equals(viewModel.EyeColor));
            var movies = new List<Movie>();
            foreach (string title in viewModel.Movies)
            {
                movies.Add(await _unitOfWork.MovieRepository.SingleOrDefaultAsync(e => e.Title.Equals(title)) ?? new Movie { Title = title });
            }

            var character = _mapping.Map<EditViewModel, Character>(viewModel, characterToUptate);
            character.Planet = planet ?? character.Planet;
            character.Race = race ?? character.Race;
            character.HairColor = hairColor ?? character.HairColor;
            character.EyeColor = eyeColor ?? character.EyeColor;
            character.Movies = movies;

            _unitOfWork.CharacterRepository.Update(characterToUptate);

            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        viewModel.PlanetList = (await _unitOfWork.PlanetRepository.GetAsync()).Select(p => p.Name).ToList();
        viewModel.RaceList = (await _unitOfWork.RaceRepository.GetAsync()).Select(r => r.Name).ToList();
        viewModel.HairColorList = (await _unitOfWork.HairColorRepository.GetAsync()).Select(h => h.Name).ToList();
        viewModel.EyeColorList = (await _unitOfWork.EyeColorRepository.GetAsync()).Select(e => e.Name).ToList();
        viewModel.MoviesList = (await _unitOfWork.MovieRepository.GetAsync()).Select(m => m.Title).ToList();

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

        var character = await _unitOfWork.CharacterRepository.SingleOrDefaultAsync(c => c.Name.Equals(name));
        if (character is not null)
        {
            _unitOfWork.CharacterRepository.Delete(character);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}

