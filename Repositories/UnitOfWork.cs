using StarWars.Data;
using StarWars.Models;

namespace StarWars.Repositories;

public class UnitOfWork
{
    private GenericRepository<Character> characterRepository;
    private GenericRepository<Planet> planetRepository;
    private GenericRepository<Race> raceRepository;
    private GenericRepository<HairColor> hairColorRepository;
    private GenericRepository<EyeColor> eyeColorRepository;
    private GenericRepository<Movie> movieRepository;

    internal StarWarsContext _context;

    public GenericRepository<Character> CharacterRepository
    {
        get
        {

            if (this.characterRepository == null)
            {
                this.characterRepository = new GenericRepository<Character>(_context);
            }
            return characterRepository;
        }
    }

    public GenericRepository<Planet> PlanetRepository
    {
        get
        {

            if (this.planetRepository == null)
            {
                this.planetRepository = new GenericRepository<Planet>(_context);
            }
            return planetRepository;
        }
    }

    public GenericRepository<Race> RaceRepository
    {
        get
        {

            if (this.raceRepository == null)
            {
                this.raceRepository = new GenericRepository<Race>(_context);
            }
            return raceRepository;
        }
    }

    public GenericRepository<HairColor> HairColorRepository
    {
        get
        {

            if (this.hairColorRepository == null)
            {
                this.hairColorRepository = new GenericRepository<HairColor>(_context);
            }
            return hairColorRepository;
        }
    }

    public GenericRepository<EyeColor> EyeColorRepository
    {
        get
        {

            if (this.eyeColorRepository == null)
            {
                this.eyeColorRepository = new GenericRepository<EyeColor>(_context);
            }
            return eyeColorRepository;
        }
    }

    public GenericRepository<Movie> MovieRepository
    {
        get
        {

            if (this.movieRepository == null)
            {
                this.movieRepository = new GenericRepository<Movie>(_context);
            }
            return movieRepository;
        }
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}