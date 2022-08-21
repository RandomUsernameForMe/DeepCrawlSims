using DeepCrawlSims;
using DeepCrawlSims.AI;
using DeepCrawlSims.BattleControl;
using DeepCrawlSims.PartyNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading.Tasks;


/// <summary>
/// Simulationrunner is the main handler of performing simulations of combat encounters.
/// </summary>
public class SimulationRunner
{
    public BattleManager manager;
    public int simCount = 100;
    public bool finished = false;

    public SimulationRunner(BattleManager manager, int simCount)
    {
        this.manager = manager;
        this.simCount = simCount;
    }

    internal async void Run()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var results = new List<BattleResults>();

        int taskCount = simCount / 100 + 1;
        // We create a new task for every 100 simulated battles. 
        for (int i = 0; i < taskCount; i++)
        {
            var result = await Task.Run(() => RunBattles(manager,Math.Min(simCount-i*100,100)));            
            results.AddRange(result);                     
        }

        // Show output after simulations have finished
        ShowResultsSummary(results);
        watch.Stop();
        Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Press Any Key to continue");
        Console.ReadLine();
        finished = true;
    }

    private List<BattleResults> RunBattles(BattleManager manager,int battleCount)
    {
        // Due to using multiple threads to perform simulations, we require hard copies of BattleManager
        // (De)serialization is a way to make such copy
        var b = new BinaryFormatter();
        var ms = new MemoryStream();
        b.Serialize(ms, manager);
        ms.Position = 0;
        var newManager = (BattleManager)b.Deserialize(ms);

        var results = new List<BattleResults>();
        for (int i = 0; i < battleCount; i++)
        {
            while (newManager.isRunning == true)
            {
                newManager.RunOneTurn();
            }
            results.Add(newManager.results);
            newManager.Reset();
        }        
        return results;
    } 

    
    /// <summary>
    /// Writes to console a summary of results after all desired simulations have finished
    /// </summary>
    public void ShowResultsSummary(List<BattleResults> results)
    {
        var gamesPlayed = results.Count;
        int wins = results.Where(x => x.result.Equals(1)).Count();
        int loses = results.Where(x => x.result.Equals(2)).Count();
        float winrate = 100*((float)wins / gamesPlayed);
        Console.WriteLine(String.Format("Played Games: {0} , Winrate: {1} %", gamesPlayed, winrate));
    }
}

