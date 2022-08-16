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

public class SimulationRunner
{
    public BattleManager manager;
    public Controller AIControl;
    public int simCount = 100;
    public float targetWinrate;
    public float winrateTolerance;
    public bool finished = false;

    internal async void Run()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var results = new List<BattleResults>();
        for (int i = 0; i < simCount; i+=100)
        {
            var result = await Task.Run(() => RunBattles(manager,100));            
            results.AddRange(result);                     
        }
        ShowResults(results);
        watch.Stop();
        Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Press Any Key to continue");
        Console.ReadLine();
        finished = true;
    }

    private List<BattleResults> RunBattles(BattleManager manager,int battleCount)
    {
        var b = new BinaryFormatter();
        var ms = new MemoryStream();
        b.Serialize(ms, manager);
        ms.Position = 0;
        var newManager = (BattleManager)b.Deserialize(ms);

        var results = new List<BattleResults>();
        for (int i = 0; i < battleCount; i++)
        {
            while (newManager.running == true)
            {
                newManager.RunOneTurn();
            }
            results.Add(newManager.results);
            newManager.Reset();
        }        
        return results;

    } 

    public SimulationRunner(BattleManager manager, int simCount)
    {
        this.manager = manager;
        this.simCount = simCount;
    }

    public void ShowResults(List<BattleResults> results)
    {
        var gamesPlayed = results.Count;

        int wins = results.Where(x => x.result.Equals(1)).Count();
        int loses = results.Where(x => x.result.Equals(2)).Count();
        float winrate = 100*((float)wins / gamesPlayed);
        Console.WriteLine(String.Format("Played Games: {0} , Winrate: {1} %", gamesPlayed, winrate));
    }

    public IEnumerator StartSimulationCoroutine()
    {
        bool finished = false;
        while(!finished)
        {
            var results = new List<BattleResults>();
            for (int i = 0; i < simCount; i++)
            {
                while (manager.running == true)
                {
                    manager.RunOneTurn();
                }
                results.Add(manager.results);

                //ShowResults(results); Not needed
                manager.Reset();
                yield return null;
            }
            finished = AreSimulationsSatisfied(results);
            if (!finished)
            {
                var points = CalculateUpgradePoints(results);
                
                //manager.enemyParty = modifier.ModifyPartyDifficulty(manager.enemyParty, points);
                manager.Reset();                
            }
        }        
    }

    private bool AreSimulationsSatisfied(List<BattleResults> results)
    {
        int wins = results.Where(x => x.result.Equals(1)).Count();
        int loses = results.Where(x => x.result.Equals(2)).Count();
        float winrate = 100 * ((float)wins / (wins + loses));
        //Debug.Log(String.Format("Winrate: {0}", winrate));
        return (Math.Abs(targetWinrate - winrate) < winrateTolerance);
    }

    public int CalculateUpgradePoints(List<BattleResults> results)
    {
        int wins = results.Where(x => x.result.Equals(1)).Count();
        int loses = results.Where(x => x.result.Equals(2)).Count();
        float winrate = 100 * ((float)wins / (wins + loses));
        return (int) (winrate-targetWinrate)/3; //změnit počet bodů zde
    }
}

