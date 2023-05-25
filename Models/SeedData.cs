using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StarWars.Data;
using StarWars.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StarWars.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using(var context = new StarWarsContext(serviceProvider.GetRequiredService<DbContextOptions<StarWarsContext>>()))
            {
                if(context.Character.Any())
                {
                    return;
                }

                Movie firstMovie = new Movie { Title = "Эпизод I: Скрытая угроза" }; // Year = 1999
                Movie secondMovie = new Movie { Title = "Эпизод II: Атака клонов" }; // Year = 2002
                Movie thirdMovie = new Movie { Title = "Эпизод III: Месть ситхов" }; // Year = 2005
                Movie fourthMovie = new Movie { Title = "Эпизод IV: Новая надежда" }; // Year = 1977
                Movie fifthMovie = new Movie { Title = "Эпизод V: Империя наносит ответный удар" }; // Year = 1980
                Movie sixthMovie = new Movie { Title = "Эпизод VI: Возвращение джедая" }; // Year = 1983
                Movie seventhMovie = new Movie { Title = "Пробуждение силы" }; // Year = 2015
                Movie eighthMovie = new Movie { Title = "Последние джедаи" }; // Year = 2017
                Movie ninthMovie = new Movie { Title = "Звездные войны: Возвышение Скайуокера" }; // Year = 2019
                context.Movie.AddRange(firstMovie, secondMovie, thirdMovie, fourthMovie, fifthMovie, sixthMovie, seventhMovie, eighthMovie, ninthMovie);

                Race unknownRace = new Race { Name = "Неизвестная раса" };
                Race humanRace = new Race { Name = "Человек" };
                Race yodaRace = new Race { Name = "Раса Йоды" };
                context.Race.AddRange(
                    unknownRace,
                    humanRace,
                    yodaRace
                );

                Planet unknownPlanet = new Planet { Name = "Неизвестная планета" };
                Planet alderaanPlanet = new Planet { Name = "Алдераан" };
                Planet tatooinePlanet = new Planet { Name = "Татуин" };
                Planet corelliaPlanet = new Planet { Name = "Кореллия" };
                Planet dagobaPlanet = new Planet { Name = "Дагоба" };
                context.Planet.AddRange(
                    unknownPlanet,
                    alderaanPlanet,
                    tatooinePlanet,
                    corelliaPlanet,
                    dagobaPlanet
                );

                HairColor unknownHairColor = new HairColor { Name = "Неизвестно" };
                context.HairColor.AddRange(unknownHairColor);

                EyeColor unknownEyeColor = new EyeColor { Name = "Неизвестно" };
                context.EyeColor.AddRange(unknownEyeColor);

                context.Character.AddRange(
                    new Character
                    {
                        Name = "Энакин Скайуокер",
                        OriginalName = "Anakin Skywalker",
                        BirthDate = 41,
                        Planet = tatooinePlanet,
                        Gender = Gender.Male,
                        Race = humanRace,
                        Height = 1.88M,
                        HairColor = new HairColor { Name = "Русые" },
                        EyeColor = new EyeColor { Name = "Голубой" },
                        History = "Энакин Скайуокер (англ. Anakin Skywalker, сокращённо Эни) — легендарный чувствительный к Силе человек, мужчина, который служил Галактической Республике как рыцарь-джедай, позже служивший Галактической Империи и командовавший её войсками, как Лорд ситхов Дарт Вейдер. Рождённый Шми Скайуокер, в юности стал тайным мужем сенатора с Набу, Падме Амидалы Наберри. Он был отцом гранд-мастера Люка Скайуокера, рыцаря-джедая Леи Органы-Соло и дедом Бена Скайуокера. Далёкими потомками Энакина Скайуокера были Нат, Кол и Кейд Скайуокеры.",
                        Movies = new List<Movie> {
                            firstMovie,
                            secondMovie,
                            thirdMovie,
                            fourthMovie,
                            fifthMovie,
                            sixthMovie
                        }
                    },
                    new Character
                    {
                        Name = "Лея Органа",
                        OriginalName = "Leia Organa",
                        BirthDate = 19,
                        Planet = alderaanPlanet,
                        Gender = Gender.Female,
                        Race = humanRace,
                        Height = 1.5M,
                        HairColor = new HairColor { Name = "Тёмно-каштановый"},
                        EyeColor = new EyeColor { Name = "Карие" },
                        History = "Лея Органа-Соло (англ. Leia Organa Solo) (имя при рождении — Лея Амидала Скайуокер) — дочь рыцаря-джедая Энакина Скайуокера и сенатора Падме Амидалы Наберри, а также сестра-близнец Люка Скайуокера. После рождения её удочерили Бейл Органа и королева Бреха, сделав её принцессой Альдераана. Получившая прекрасное образование сенатора, Органа была известна, как непоколебимый лидер во время Галактической гражданской войны и других последующих галактических конфликтов, став одним из величайших героев Галактики. Позднее она вышла замуж за Хана Соло и стала матерью троих детей: Джейны, Джейсена и Энакина. Незадолго до начала Роевой войны, Лея, сама того не зная, стала бабушкой дочери Джейсена — Алланы.",
                        Movies = new List<Movie> {
                            thirdMovie,
                            fourthMovie,
                            fifthMovie,
                            sixthMovie,
                            seventhMovie,
                            eighthMovie,
                            ninthMovie
                        }
                    },
                    new Character
                    {
                        Name = "Йода",
                        OriginalName = "Yoda",
                        BirthDate = 896,
                        Planet = dagobaPlanet,
                        Gender = Gender.Male,
                        Race = yodaRace,
                        Height = 0.66M,
                        HairColor = new HairColor { Name = "Белый"},
                        EyeColor = new EyeColor { Name = "Зеленый" },
                        History = "Йода (англ. Yoda) — гранд-мастер Ордена джедаев, был одним из самых сильных и мудрых джедаев своего времени. Место в Совете получил спустя примерно сотню лет после рождения. Обладая долголетием, он достиг титула гранд-мастера в возрасте примерно 600 лет. Йода сумел выжить во время приказа 66. После неудачной дуэли с Дартом Сидиусом ушел в добровольное изгнание на планету Дагоба, где и умер естественной смертью в 4 ПБЯ. Родная планета и раса Йоды неизвестны."
                                + "\nМагистр Йода был одним из сильнейших джедаев своего времени. Он был самым мудрым из них. Во владении световым мечом с Йодой могли сравниться только Мейс Винду, Энакин Скайуокер, Оби-Ван Кеноби, граф Дуку и Дарт Сидиус.",
                        Movies = new List<Movie> {
                            firstMovie,
                            thirdMovie,
                            fifthMovie,
                            sixthMovie,
                            seventhMovie,
                            eighthMovie,
                            ninthMovie
                        }
                    },
                    new Character
                    {
                        Name = "Хан Соло",
                        OriginalName = "Han Solo",
                        BirthDate = 29,
                        Planet = corelliaPlanet,
                        Gender = Gender.Male,
                        Race = humanRace,
                        Height = 1.8M,
                        HairColor = new HairColor { Name = "Каштановый" },
                        EyeColor = new EyeColor { Name = "Карий" },
                        History = "Хан Соло (англ. Han Solo) — знаменитый кореллианин, прославившийся на всю Галактику как участник Восстания против Галактической Империи. Родителями Хана были Джонаш и Джейна Соло, которые исчезли когда ему было не больше двух лет от роду. Его отец был принцем и потомком короля Беретрона э Соло, который установил демократический режим в Кореллианской Империи. У Хана было тяжелое детство, но он сумел порвать с прошлым и поступить на службу в Империю. Крест на своей карьере Соло поставил, когда заступился за раба-вуки Чубакку. Вместе они сбежали и со временем стали напарниками. Хан приобрел корабль «Тысячелетний сокол» и стал контрабандистом.",
                        Movies = new List<Movie> {
                            fourthMovie,
                            fifthMovie,
                            sixthMovie,
                            seventhMovie,
                            ninthMovie
                        }
                    }
                );
                context.SaveChanges();
            }
        }
    }
}