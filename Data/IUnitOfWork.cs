using StarWars.Models;

namespace StarWars.Data;

public interface IUnitOfWork{

    IGenericRepository<Character> CharacterRepository { get; }
    IGenericRepository<Planet> PlanetRepository { get; }
    IGenericRepository<Race> RaceRepository { get; }
    IGenericRepository<HairColor> HairColorRepository { get; }
    IGenericRepository<EyeColor> EyeColorRepository { get; }
    IGenericRepository<Movie> MovieRepository { get; }
    
    Task<int> SaveAsync();
}