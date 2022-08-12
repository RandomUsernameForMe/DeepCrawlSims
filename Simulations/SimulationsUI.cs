using DeepCrawlSims.BattleControl;
using DeepCrawlSims.PartyNamespace;
using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCrawlSims.Simulations
{
    class SimulationsUI
    {
        BattleManager battleManager;

        public SimulationsUI(BattleManager battleManager)
        {
            this.battleManager = battleManager;
        }

        public void Run()
        {
            ShowPartyToConsole(battleManager.allyParty);
            ShowPartyToConsole(battleManager.enemyParty);

        }

        public void ShowPartyToConsole(Party party)
        {
            foreach (var creature in party.Creatures)
            {
                Console.WriteLine();
                Console.WriteLine("Name: {0}", creature.name);
                Query query = new Query(QueryType.Description);
                query.Add(QueryParameter.Tooltip,0);
                string description = string.Join("\n",creature.ProcessQuery(query).descs);
                Console.WriteLine(description);
            }
            
        }

    }
}
