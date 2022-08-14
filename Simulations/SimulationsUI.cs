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
        const int RUN_CONST = 3;

        BattleManager battleManager;
        int command;
        Party currentParty;
        Creature currentCreature;

        public SimulationsUI(BattleManager battleManager)
        {
            this.battleManager = battleManager;
        }

        public void Run()
        {
            while (command != 0)
            {
                switch (command)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 1:
                        break;
                    case 1:
                        break;
                    case 1:
                        break;
                    default:
                }
            }
            ShowPartyToConsole(battleManager.allyParty);
            ShowPartyToConsole(battleManager.enemyParty);

        }

        public void ShowPartyToConsole(Party party)
        {
            Console.Clear();
            foreach (var creature in party.Creatures)
            {
                Console.WriteLine();
                Console.WriteLine("Name: {0}", creature.name);
                Query query = new Query(QueryType.Description);
                query.Add(QueryParameter.Tooltip,0);
                string description = string.Join("\n",creature.ProcessQuery(query).descs);
                Console.WriteLine(description);
            }

            Console.WriteLine("What do you want to do now? (enter apropriate number)");
            Console.WriteLine("1-4 = Modify party member");
            Console.WriteLine("5 = Go back");

            int input = 0;
            while (input == 0)
            {
                input = Console.ReadLine()[0] - '0';
            }

            if (input >0 && input <5)
            {
                ShowUpgradesToConsole(party.Creatures[input - 1]);
            }
            
        }
        public void ShowMainMenuToConsole(Party party)
        {
            Console.Clear();
            Console.WriteLine("What do you want to do now? (enter apropriate number)");
            Console.WriteLine("1 = Show/modify first party");
            Console.WriteLine("2 = Show/show second party");
            Console.WriteLine("3 = Modify number of simulations");
            Console.WriteLine("4 = Run simulations");

            int input=0;
            while (input == 0)
            {
                input = Console.ReadLine()[0] -'0';
            }

            switch (input)
            {
                case 1:
                    ShowPartyToConsole(battleManager.allyParty);
                    break;
                case 2:
                    ShowPartyToConsole(battleManager.enemyParty);
                    break;
                case 3:
                    ShowPartyToConsole(battleManager.allyParty); //TODO
                    break;
                case 4:
                    command = RUN_CONST;
                    break;
            }


        }

        public void ShowUpgradesToConsole(Creature creature)
        {
            foreach (var item in creature.components)
            {
                if 
            }
        }
    }
}
