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
            Party allyParty;
            Party enemyParty;

            allyParty = TeamBuilder.CreateAllyTeam();
            enemyParty = TeamBuilder.CreateEnemyTeam();
            PartySerializer.Serialize(allyParty, "Setups/defaultAlly");
            PartySerializer.Serialize(enemyParty, "Setups/defaultEnemy");

            BattleManager manager = new BattleManager(allyParty, enemyParty);
            SimulationsUI sim = new SimulationsUI(manager);
            sim.Run();
        }
    }
}
