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
            var allyParty = TeamBuilder.CreateTeamOne();
            var enemyParty = TeamBuilder.CreateTeamTwo().MakeOppositeSide(true);
            System.IO.Directory.CreateDirectory("Setups");
            PartySerializer.Serialize(allyParty, "Setups/defaultAlly");
            PartySerializer.Serialize(enemyParty, "Setups/defaultEnemy");

            var manager = new BattleManager(allyParty, enemyParty);
            var controller = new SimulationsUIAndControl(manager);
            controller.RunUI();
        }
    }
}
