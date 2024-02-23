using BracketGenerator.Models;
using BracketGenerator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

var host = Host.CreateDefaultBuilder(args)
   .ConfigureServices((hostContext, services) =>
   {
       services.AddTransient<ITournamentBracketService, SingleEliminationBracketService>();
   }).Build();

var bracketService = host.Services.GetService<ITournamentBracketService>();

try
{
    string currentDirectory = Directory.GetCurrentDirectory();
    string seedFilePath = Path.Combine(currentDirectory, "../../../Data/SeedFile.json");
    string winnersFilePath = Path.Combine(currentDirectory, "../../../Data/Winners.json");

    string seedString = File.ReadAllText(seedFilePath);
    dynamic seedFile = JsonConvert.DeserializeObject(seedString);

    string winnersString = File.ReadAllText(winnersFilePath);
    dynamic winnersFile = JsonConvert.DeserializeObject(winnersString);

    //Change here R32 or R16 ro run both cases
    List<SeedTeam> seedTeams = seedFile["R32"].ToObject<List<SeedTeam>>();

    if (bracketService is not null)
    {
        bracketService.SeedTeam(seedTeams);
        bool isNotFinished = true;
        int round = 1;

        do
        {
            bracketService.GenerateMatches();

            //Change here R32 or R16 ro run both cases
            List<WinningTeam> winningTeams = winnersFile[$"R32_Round{round}"].ToObject<List<WinningTeam>>();

            if (winningTeams == null || winningTeams.Count == 1)
            {
                isNotFinished = false;
            }

            foreach (var team in winningTeams)
            {
                bracketService.AdvanceTeam(team.Team);
            }
            round++;

        } while (isNotFinished);

        Console.WriteLine(bracketService.GetTournamentWinner());
        Console.WriteLine(bracketService.PathToVictory());
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

