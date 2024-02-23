using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BracketGenerator.Models
{
    public interface ITournamentMatch
    {
        int MatchId { get; set; }
        string Team1 { get; }
        string Team2 { get; }
        string Winner { get; set; }
        bool IsFinished { get; set; }
        public int Round { get; set; }
        public string Group { get; set; }
    }

    public class TournamentMatch : ITournamentMatch
    {
        public int MatchId { get; set; }
        public string Team1 { get; }
        public string Team2 { get; }
        public string Winner { get; set; }
        public int Round { get; set; }
        public bool IsFinished { get; set; }
        public string Group { get; set; }

        public TournamentMatch(string team1, string team2)
        {
            Team1 = team1;
            Team2 = team2;
            Winner = String.Empty;
            Group = String.Empty;
            IsFinished = false;
        }

    }
}
