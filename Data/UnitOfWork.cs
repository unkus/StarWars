using StarWars.Models;

namespace StarWars.Data;

public class UnitOfWork
{
    // Isn't the same thing being done in StarWarsContext? Is this relevant for the latest version of EF?

    private Lazy<GenericRepository<Character>> _characterRepository;
    private Lazy<GenericRepository<Planet>> _planetRepository;
    private Lazy<GenericRepository<Race>> _raceRepository;
    private Lazy<GenericRepository<HairColor>> _hairColorRepository;
    private Lazy<GenericRepository<EyeColor>> _eyeColorRepository;
    // Другой путь инициализации. 
    // TODO: Какой продуктивнее?
    private GenericRepository<Movie>? _movieRepository;
    private StarWarsContext? _context;

    public UnitOfWork(StarWarsContext context) {
        _context = context;

        _characterRepository = new Lazy<GenericRepository<Character>>(() => new GenericRepository<Character>(context));
        _planetRepository = new Lazy<GenericRepository<Planet>>(() => new GenericRepository<Planet>(context));
        _raceRepository = new Lazy<GenericRepository<Race>>(() => new GenericRepository<Race>(context));
        _hairColorRepository = new Lazy<GenericRepository<HairColor>>(() => new GenericRepository<HairColor>(context));
        _eyeColorRepository = new Lazy<GenericRepository<EyeColor>>(() => new GenericRepository<EyeColor>(context));
    }

    public GenericRepository<Character> CharacterRepository
    {
        get => _characterRepository.Value;
    }

    public GenericRepository<Planet> PlanetRepository
    {
        get => _planetRepository.Value;
    }

    public GenericRepository<Race> RaceRepository
    {
        get => _raceRepository.Value;
    }

    public GenericRepository<HairColor> HairColorRepository
    {
        get => _hairColorRepository.Value;
    }

    public GenericRepository<EyeColor> EyeColorRepository
    {
        get => _eyeColorRepository.Value;
    }

    public GenericRepository<Movie> MovieRepository
    {
        get => _movieRepository = _movieRepository ?? new GenericRepository<Movie>(_context);
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
}