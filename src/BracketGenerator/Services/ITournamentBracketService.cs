using BracketGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BracketGenerator.Services
{
    public interface ITournamentBracketService
    {
        void SeedTeam(List<SeedTeam> seedTeams);
        void GenerateMatches();
        void AdvanceTeam(string winningTeam);
        bool IsTournamentFinished();
        string GetTournamentWinner();
        string PathToVictory();
        List<TournamentMatch> GetMatches();
        List<SeedTeam> GetTeams();
    }
}
