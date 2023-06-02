using AutoMapper;
using StarWars.Models;
using StarWars.ViewModels;

namespace StarWars.Mapping;

public class StarWarsMappingProfile : Profile
{

    public StarWarsMappingProfile()
    {
        CreateMap<Character, CardViewModel>();

        CreateMap<Character, DetailsViewModel>()
            .IncludeBase<Character, CardViewModel>()
            .ForMember(view => view.Planet, opt => opt.MapFrom(character => character.Planet.Name))
            .ForMember(view => view.Race, opt => opt.MapFrom(character => character.Race.Name))
            .ForMember(view => view.HairColor, opt => opt.MapFrom(character => character.HairColor.Name))
            .ForMember(view => view.EyeColor, opt => opt.MapFrom(character => character.EyeColor.Name))
            .ForMember(view => view.Movies, opt => opt.MapFrom(character => character.Movies.Select(m => m.Title).ToList()));

        CreateMap<Character, CreateViewModel>()
            .IncludeBase<Character, DetailsViewModel>();

        CreateMap<Character, EditViewModel>()
            .IncludeBase<Character, CreateViewModel>();

		// TODO: Is it correct way? Should/Can I call database here?
		// Expected action: get Planet from database or create new if not exist.
        CreateMap<CreateViewModel, Character>()
            .ForMember(character => character.Planet, opt => opt.MapFrom(view => new Planet { Name = view.Planet }))
            .ForMember(character => character.Race, opt => opt.MapFrom(view => new Race { Name = view.Race }))
            .ForMember(character => character.HairColor, opt => opt.MapFrom(view => new HairColor { Name = view.HairColor }))
            .ForMember(character => character.EyeColor, opt => opt.MapFrom(view => new EyeColor { Name = view.EyeColor }))
            .ForMember(character => character.Movies, opt => opt.MapFrom(view => view.Movies.Select(title => new Movie { Title = title}).ToList()));

        CreateMap<EditViewModel, Character>()
            .IncludeBase<CreateViewModel, Character>();
    }
}