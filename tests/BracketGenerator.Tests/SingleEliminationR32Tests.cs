using BracketGenerator.Models;
using BracketGenerator.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace BracketGenerator.Tests;

public class SingleEliminationR32Tests
{
    private readonly SingleEliminationBracketService _tournamentBracketService;

    public SingleEliminationR32Tests()
    {
        _tournamentBracketService =  new SingleEliminationBracketService();
    }

    [Theory]
    [InlineData(32,"1A","Netherlands")]
    public void SeedTeams_Team_And_Count_Tests(int count, string seed, string team)
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);
        // Already seed done in constructor. need to check count here
        var teams = _tournamentBracketService.GetTeams();
        Assert.Equal(count, teams.Count);
        Assert.Equal(seed, teams.ElementAt(0).Seed);
        Assert.Equal(team, teams.ElementAt(0).Team);
    }

    [Fact]
    public void SeedTeam_Invalid__Count_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = new ();
        Action action = () => _tournamentBracketService.SeedTeam(seedTeams);
        ArgumentException exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("No teams to seed", exception.Message);
    }

    [Fact]
    public void GenerateMatches_Invalid_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = new List<SeedTeam>() { new SeedTeam("1A", "Netherlands") };
        _tournamentBracketService.SeedTeam(seedTeams);

        Action action = () => _tournamentBracketService.GenerateMatches();
        ArgumentException exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("The number of teams must be even and Greater than 0", exception.Message);
    }

    [Fact]
    public void GenerateMatches_Invalid_With_Zero_Teams_Tests()
    {
        Action action = () => _tournamentBracketService.GenerateMatches();
        ArgumentException exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("The number of teams must be even and Greater than 0", exception.Message);
    }
    
    [Fact]
    public void GenerateMatches_Without_Completing_Round_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);
        _tournamentBracketService.GenerateMatches();

        Action action = () => _tournamentBracketService.GenerateMatches();
        ArgumentException exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("Round is not completed yet..", exception.Message);
    }

    [Fact]
    public void GenerateMatches_Count_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);

        _tournamentBracketService.GenerateMatches();
        List<TournamentMatch> matches = _tournamentBracketService.GetMatches();

        Assert.Equal(16, matches.Count);
        Assert.True(matches.FindAll(m => m.IsFinished).Count == 0);
    }


    [Fact]
    public void GenerateMatches_First_Round_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);

        //Generate first round with teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams = GetWinningTeams(1);
        foreach (var team in winningTeams)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate second round with winning teams
        _tournamentBracketService.GenerateMatches();

        List<TournamentMatch> matches = _tournamentBracketService.GetMatches();

        Assert.Equal(24, matches.Count);
        Assert.True(matches.FindAll(m => m.IsFinished).Count == 16);
        Assert.True(matches.FindAll(m => !m.IsFinished).Count == 8);
        Assert.False(_tournamentBracketService.IsTournamentFinished());
    }

    [Fact]
    public void GenerateMatches_Second_Round_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);

        //Generate first round with teams
        _tournamentBracketService.GenerateMatches();
        

        //Update winning teams
        List<WinningTeam> winningTeams1 = GetWinningTeams(1);
        foreach (var team in winningTeams1)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate second round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams2 = GetWinningTeams(2);
        foreach (var team in winningTeams2)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate third round with winning teams
        _tournamentBracketService.GenerateMatches();

        List<TournamentMatch> matches = _tournamentBracketService.GetMatches();

        Assert.Equal(28, matches.Count);
        Assert.True(matches.FindAll(m => m.IsFinished).Count == 24);
        Assert.True(matches.FindAll(m => !m.IsFinished).Count == 4);
        Assert.False(_tournamentBracketService.IsTournamentFinished());
    }


    [Fact]
    public void GenerateMatches_Third_Round_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);

        //Generate first round with teams
        _tournamentBracketService.GenerateMatches();


        //Update winning teams
        List<WinningTeam> winningTeams1 = GetWinningTeams(1);
        foreach (var team in winningTeams1)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate second round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams2 = GetWinningTeams(2);
        foreach (var team in winningTeams2)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate third round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams3 = GetWinningTeams(3);
        foreach (var team in winningTeams3)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate fourth round with winning teams
        _tournamentBracketService.GenerateMatches();

        List<TournamentMatch> matches = _tournamentBracketService.GetMatches();

        Assert.Equal(30, matches.Count);
        Assert.True(matches.FindAll(m => m.IsFinished).Count == 28);
        Assert.True(matches.FindAll(m => !m.IsFinished).Count == 2);
        Assert.False(_tournamentBracketService.IsTournamentFinished());
    }

    [Fact]
    public void GenerateMatches_Fourth_Round_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);

        //Generate first round with teams
        _tournamentBracketService.GenerateMatches();


        //Update winning teams
        List<WinningTeam> winningTeams1 = GetWinningTeams(1);
        foreach (var team in winningTeams1)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate second round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams2 = GetWinningTeams(2);
        foreach (var team in winningTeams2)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate third round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams3 = GetWinningTeams(3);
        foreach (var team in winningTeams3)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate fourth round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams4 = GetWinningTeams(4);
        foreach (var team in winningTeams4)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate fourth round with winning teams
        _tournamentBracketService.GenerateMatches();

        List<TournamentMatch> matches = _tournamentBracketService.GetMatches();

        Assert.Equal(31, matches.Count);
        Assert.True(matches.FindAll(m => m.IsFinished).Count == 30);
        Assert.True(matches.FindAll(m => !m.IsFinished).Count == 1);
        Assert.False(_tournamentBracketService.IsTournamentFinished());
    }

    [Fact]
    public void GenerateMatches_Last_Round_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);

        //Generate first round with teams
        _tournamentBracketService.GenerateMatches();


        //Update winning teams
        List<WinningTeam> winningTeams1 = GetWinningTeams(1);
        foreach (var team in winningTeams1)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate second round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams2 = GetWinningTeams(2);
        foreach (var team in winningTeams2)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate third round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams3 = GetWinningTeams(3);
        foreach (var team in winningTeams3)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate fourth round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams4 = GetWinningTeams(4);
        foreach (var team in winningTeams4)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate fourth round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams5 = GetWinningTeams(5);
        foreach (var team in winningTeams5)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        List<TournamentMatch> matches = _tournamentBracketService.GetMatches();

        Assert.Equal(31, matches.Count);
        Assert.True(matches.FindAll(m => m.IsFinished).Count == 31);
        Assert.True(matches.FindAll(m => !m.IsFinished).Count == 0);

        //Tournament is finished here
        Assert.True(_tournamentBracketService.IsTournamentFinished());
    }

    [Fact]
    public void GenerateMatches_Invalid_Last_Round_Advance_Team_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);

        //Generate first round with teams
        _tournamentBracketService.GenerateMatches();


        //Update winning teams
        List<WinningTeam> winningTeams1 = GetWinningTeams(1);
        foreach (var team in winningTeams1)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate second round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams2 = GetWinningTeams(2);
        foreach (var team in winningTeams2)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate third round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams3 = GetWinningTeams(3);
        foreach (var team in winningTeams3)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate fourth round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams4 = GetWinningTeams(4);
        foreach (var team in winningTeams4)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate fourth round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams5 = GetWinningTeams(5);
        foreach (var team in winningTeams5)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        Action action = () => _tournamentBracketService.AdvanceTeam("Netherland");
        ArgumentException exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("Team not found in any match or match is already finished.", exception.Message);
    }

    [Fact]
    public void GetTournamentWinner_Invalid_Tournement_Not_Finished_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);

        _tournamentBracketService.GenerateMatches();

        Action action = () => _tournamentBracketService.GetTournamentWinner();
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("The tournament is not finished yet.", exception.Message);
    }

    

    [Theory]
    [InlineData("Taiwan")]
    public void AdvanceTeam_Invalid_Team_Name_Tests(string teamName)
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);

        //Generate first round with teams
        _tournamentBracketService.GenerateMatches();
        List<TournamentMatch> matches = _tournamentBracketService.GetMatches();

        //Update winning teams with team not in the tournement
        Assert.Throws<ArgumentException>(() => _tournamentBracketService.AdvanceTeam(teamName));
    }

    [Fact]
    public void AdvanceTeam_Invalid_With_Invalid_Team_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);

        //Generate first round with teams
        _tournamentBracketService.GenerateMatches();

        Action action = () => _tournamentBracketService.AdvanceTeam("Ireland");
        ArgumentException exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("Team not found in any match or match is already finished.", exception.Message);
    }

    [Theory]
    [InlineData(31,"Netherlands", "Netherlands", "France", "Ecuador,Quatar,Argentina,Germany,France")]
    public void PathToVictory_Last_Round_And_Winning_Path_Tests(int matchid,string winningTeam, string team1, string team2,string path)
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);

        //Generate first round with teams
        _tournamentBracketService.GenerateMatches();


        //Update winning teams
        List<WinningTeam> winningTeams1 = GetWinningTeams(1);
        foreach (var team in winningTeams1)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate second round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams2 = GetWinningTeams(2);
        foreach (var team in winningTeams2)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate third round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams3 = GetWinningTeams(3);
        foreach (var team in winningTeams3)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate fourth round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams4 = GetWinningTeams(4);
        foreach (var team in winningTeams4)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        //Generate fourth round with winning teams
        _tournamentBracketService.GenerateMatches();

        //Update winning teams
        List<WinningTeam> winningTeams5 = GetWinningTeams(5);
        foreach (var team in winningTeams5)
        {
            _tournamentBracketService.AdvanceTeam(team.Team);
        }

        List<TournamentMatch> matches = _tournamentBracketService.GetMatches();

        Assert.Equal(31, matches.Count);
        Assert.True(matches.FindAll(m => m.IsFinished).Count == 31);
        Assert.True(matches.FindAll(m => !m.IsFinished).Count == 0);
        Assert.Equal(winningTeam,matches.Last().Winner);
        Assert.Equal(team1, matches.Last().Team1);
        Assert.Equal(team2, matches.Last().Team2);
        Assert.Equal("", matches.Last().Group);
        Assert.Equal(31, matches.Last().MatchId);
        //Tournament is finished here
        Assert.True(_tournamentBracketService.IsTournamentFinished());
        Assert.Equal(_tournamentBracketService.GetTournamentWinner(), winningTeam);
        Assert.Equal(_tournamentBracketService.PathToVictory(), path);
    }

    [Fact]
    public void PathToVictory_Invalid_Not_Completed_Tests()
    {
        //Seed teams
        List<SeedTeam> seedTeams = GetSeedTeams();
        _tournamentBracketService.SeedTeam(seedTeams);

        _tournamentBracketService.GenerateMatches();

        Action action = () => _tournamentBracketService.PathToVictory();
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("The tournament is not finished yet.", exception.Message);
    }

    private List<WinningTeam> GetWinningTeams(int round)
    {
        string winnersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Data/Winners.json");
        string winnersString = File.ReadAllText(winnersFilePath);
        dynamic winnersFile = JsonConvert.DeserializeObject(winnersString);
        List<WinningTeam> winningTeams = winnersFile[$"R32_Round{round}"].ToObject<List<WinningTeam>>();
        return winningTeams;
    }

    private List<SeedTeam> GetSeedTeams()
    {
        string seedFilePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Data/SeedFile.json");
        string seedString = File.ReadAllText(seedFilePath);
        dynamic seedFile = JsonConvert.DeserializeObject(seedString);
        List<SeedTeam> seedTeams = seedFile["R32"].ToObject<List<SeedTeam>>();
        return seedTeams;
    }

}