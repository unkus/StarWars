using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StarWars.Models;

namespace StarWars.Data
{
    public class StarWarsContext : DbContext
    {
        public StarWarsContext (DbContextOptions<StarWarsContext> options)
            : base(options)
        {
        }

        public DbSet<StarWars.Models.Character> Character { get; set; } = default!;

        public DbSet<StarWars.Models.Planet> Planet { get; set; } = default!;

        public DbSet<StarWars.Models.Race> Race { get; set; } = default!;

        public DbSet<StarWars.Models.HairColor> HairColor { get; set; } = default!;

        public DbSet<StarWars.Models.EyeColor> EyeColor { get; set; } = default!;

        public DbSet<StarWars.Models.Movie> Movie { get; set; } = default!;
    }
}