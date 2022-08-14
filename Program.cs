using DeepCrawlSims.BattleControl;
using DeepCrawlSims.PartyNamespace;
using DeepCrawlSims.Simulations;
using System;

namespace DeepCrawlSims
{
    class Program
    {
        static void Main(string[] args)
        {
            Party party = TeamBuilder.CreateAllyTeam();
            PartySerializer.Serialize(party, "party.txt");

            party = PartySerializer.Deserialize("party.txt");
            Party enemyParty = TeamBuilder.CreateEnemyTeam();
            BattleManager manager = new BattleManager(party, enemyParty);

            SimulationsUI sim = new SimulationsUI(manager);
            sim.Run();

            Console.ReadLine();


            
          

        }
    }
}
 