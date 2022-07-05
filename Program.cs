using DeepCrawlSims.BattleControl;
using DeepCrawlSims.PartyNamespace;
using System;

namespace DeepCrawlSims
{
    class Program
    {
        static void Main(string[] args)
        {
            Party party = TeamBuilder.CreateAllyTeam();
            Party enemyParty = TeamBuilder.CreateEnemyTeam();
            BattleManager manager = new BattleManager(party, enemyParty);
            
            SimulationRunner simRunner = new SimulationRunner(manager, 1000);
            simRunner.Run();
           
            

            Console.ReadLine();

        }
    }
}
 