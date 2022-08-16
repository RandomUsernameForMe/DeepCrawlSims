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
        const int RUN_CONST = 2;
        const int MENU_CONST = 1;
        const int BUSY_CONST = 3;

        BattleManager manager;
        int command = MENU_CONST;

        public SimulationsUI(BattleManager battleManager)
        {
            manager = battleManager;
        }

        public void Run()
        {
            SimulationRunner simRunner = new SimulationRunner(manager, 1000);
            while (command != 0)
            {
                switch (command)
                {
                    case MENU_CONST:
                        ShowMainMenuToConsole();
                        break;
                    case RUN_CONST:                        
                        simRunner.Run();
                        command = BUSY_CONST;
                        break;
                    case BUSY_CONST:
                        if (simRunner.finished)
                        {
                            command = MENU_CONST;
                            simRunner.finished = false;
                        }
                        break;
                }
            }
        }

        public void ShowPartyToConsole(Party party)
        {
            Console.Clear();
            foreach (var creature in party.Creatures)
            {
                ShowCreatureToConsole(creature);
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

        public void ShowCreatureToConsole(Creature creature)
        {
            Console.WriteLine("Name: {0}", creature.name);
            Query query = new Query(QueryType.Description);
            query.Add(QueryParameter.Tooltip, 0);
            string description = string.Join("\n", creature.ProcessQuery(query).descs);
            Console.WriteLine(description);
            Console.WriteLine();
        }
        public void ShowMainMenuToConsole()
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
                    ShowPartyToConsole(manager.allyParty);
                    break;
                case 2:
                    ShowPartyToConsole(manager.enemyParty);
                    break;
                case 3:
                    
                    break;
                case 4:
                    command = 2;
                    break;
            }


        }

        public void ShowUpgradesToConsole(Creature creature)
        {
            Console.Clear();

            ShowCreatureToConsole(creature);
            List<UpgradeWithCondition> passed = new List<UpgradeWithCondition>();
            foreach (var item in UpgradeStorage.GetPositiveUpgrades())
            {
                item.Condition.creature = creature;
                if (item.Condition.isPassed())
                {
                    passed.Add(item);
                }
            }

            Console.WriteLine("What do you want to do now? (enter apropriate number)");
            int counter = 0;
            foreach (var item in passed)
            {
                counter++;
                Console.WriteLine("{0}:{1} ({2})", counter, item.Upgrade.buttonText, item.Upgrade.descriptionText);
            }
            counter++;
            Console.WriteLine("{0}: Go back",counter);

            int input = 0;
            while (input != -1)
            {
                input = Console.ReadLine()[0] - '0';
                if (input < counter && input > 0)
                {
                    if (passed[input - 1].TryApplyUpgrade(creature, true, true))
                    {
                        Console.WriteLine("Upgrade Succesful!");
                    }
                    else
                    {
                        Console.WriteLine("For some reason, upgrade failed.");
                    }
                }
                if (input == counter)
                {
                    input = -1;
                }                
            }            
        }
    }
}
