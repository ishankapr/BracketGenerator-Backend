
namespace BracketGenerator.Models
{
    public class SeedTeam
    {
        public string Seed { get; set; }
        public string Team { get; set; }

        public SeedTeam(string seed, string team)
        {
            Seed = seed;
            Team = team;
        }
    }
}
