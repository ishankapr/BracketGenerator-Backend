using BracketGenerator.Models;

namespace BracketGenerator.Services
{
    public class SingleEliminationBracketService : ITournamentBracketService
    {
        private readonly Dictionary<string, string> teams = new();
        private readonly List<TournamentMatch> matches = new();
        private readonly List<string> groups = new() { "A", "B" };

        public void SeedTeam(List<SeedTeam> seedTeams)
        {
            if (seedTeams.Count == 0)
            {
                throw new ArgumentException("No teams to seed");
            }

            foreach (SeedTeam team in seedTeams)
            {
                teams.Add(team.Seed, team.Team);
            }
        }

        public void GenerateMatches()
        {
            if (teams.Count % 2 != 0 || teams.Count == 0)
            {
                throw new ArgumentException("The number of teams must be even and Greater than 0");
            }

            if (matches.Count == 0)
            {
                for (int i = 0; i < teams.Count; i += 2)
                {
                    var match = new TournamentMatch(teams.ElementAt(i).Value, teams.ElementAt(i + 1).Value)
                    {
                        Round = 1,
                        MatchId = matches.Count + 1,
                        Group = (matches.Count + 1) % 2 == 0 ? "B" : "A"
                    };
                    matches.Add(match);
                }
            }
            else
            {
                if (matches.Where(m => m.IsFinished == false).Any())
                {
                    throw new ArgumentException("Round is not completed yet..");
                }

                int lastRound = matches.Last().Round;

                foreach (string group in groups)
                {
                    List<string> winners = matches.Where(m => m.Round == lastRound && m.Group == group && m.IsFinished).OrderBy(m => m.MatchId).Select(m => m.Winner).Distinct().ToList();
                    if (winners.Count > 1)
                    {
                        for (int i = 0; i < winners.Count; i += 2)
                        {
                            var match = new TournamentMatch(winners.ElementAt(i), winners.ElementAt(i + 1))
                            {
                                Round = lastRound + 1,
                                MatchId = matches.Count + 1,
                                Group = group
                            };
                            matches.Add(match);
                        }
                    }
                    else
                    {
                        string winnerA = matches.Where(m => m.Round == lastRound && m.Group == "A" && m.IsFinished).First().Winner;
                        string winnerB = matches.Where(m => m.Round == lastRound && m.Group == "B" && m.IsFinished).First().Winner;
                        var match = new TournamentMatch(winnerA, winnerB)
                        {
                            Round = lastRound + 1,
                            MatchId = matches.Count + 1,
                            Group = ""
                        };
                        matches.Add(match);
                        return;
                    }
                }

            }

        }

        public void AdvanceTeam(string winningTeam)
        {
            var match = matches.Where(m => !m.IsFinished && (m.Team1 == winningTeam || m.Team2 == winningTeam)).OrderByDescending(m => m.MatchId).FirstOrDefault();
            if (match == null)
            {
                throw new ArgumentException("Team not found in any match or match is already finished.");
            }
            match.IsFinished = true;
            match.Winner = winningTeam;
        }

        public bool IsTournamentFinished()
        {
            if (matches.Count == 0)
            {
                return false;
            }
            bool isLastMatchCreated = matches.Count % 2 != 0;
            bool isLastMatchFinished = matches.OrderByDescending(m => m.MatchId).First().IsFinished;
            return isLastMatchCreated && isLastMatchFinished;
        }

        public string GetTournamentWinner()
        {
            if (!IsTournamentFinished())
            {
                throw new InvalidOperationException("The tournament is not finished yet.");
            }
            return matches.OrderByDescending(m => m.MatchId).First().Winner;
        }

        public string PathToVictory()
        {
            if (!IsTournamentFinished())
            {
                throw new InvalidOperationException("The tournament is not finished yet.");
            }
            string winner = GetTournamentWinner();
            List<string> path = new();
            var matchList = matches.Where(m => m.Winner == winner).OrderBy(m => m.MatchId);

            foreach (var match in matchList)
            {
                string opponent = match.Team1 == winner ? match.Team2 : match.Team1;
                path.Add(opponent);
            }
            return string.Join(",", path);
        }

        public List<TournamentMatch> GetMatches()
        {
            return matches;
        }

        public List<SeedTeam> GetTeams()
        {
            return teams.Select(t => new SeedTeam(t.Key, t.Value)).ToList();
        }
    }
}
