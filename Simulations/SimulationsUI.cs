using DeepCrawlSims.BattleControl;
using DeepCrawlSims.PartyNamespace;
using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeepCrawlSims.Simulations
{
    class SimulationsUI
    {
        const int RUN_CONST = 2;
        const int MENU_CONST = 1;
        const int BUSY_CONST = 3;

        BattleManager manager;
        SimulationRunner simRunner;
        int command = MENU_CONST;

        public SimulationsUI(BattleManager battleManager)
        {
            manager = battleManager;
        }

        public void Run()
        {
            simRunner = new SimulationRunner(manager, 1000);
            while (command != 0)
            {
                switch (command)
                {
                    case MENU_CONST:
                        ShowMainMenu();
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

        public void ShowPartyMenu(Party party)
        {
            bool finished = false;
            while (!finished) {
                Console.Clear();
                foreach (var creature in party.Creatures)
                {
                    ShowCreatureMenu(creature);
                }

                Console.WriteLine("What do you want to do now? (enter apropriate number)");
                Console.WriteLine("1-4 = Modify party member");
                Console.WriteLine("5 = Go back");

                int input = 0;
                while (input == 0)
                {
                    input = Console.ReadLine()[0] - '0';
                }

                if (input > 0 && input < 5)
                {
                    ShowUpgradesMenu(party.Creatures[input - 1]);
                }
                if (input == 5) finished = true; ; 
            }
        }

        public void ShowCreatureMenu(Creature creature)
        {
            Console.WriteLine("Name: {0}", creature.name);
            Query query = new Query(QueryType.Description);
            query.Add(QueryParameter.Tooltip, 0);
            string description = string.Join("\n", creature.ProcessQuery(query).descs);
            Console.WriteLine(description);
            Console.WriteLine();
        }
        public void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("What do you want to do now? (enter apropriate number)");
            Console.WriteLine("1 = Show/modify first party");
            Console.WriteLine("2 = Show/show second party");
            Console.WriteLine("3 = Save setup to file");
            Console.WriteLine("4 = Load setup from file");
            Console.WriteLine("5 = Modify number of simulations");
            Console.WriteLine("6 = Run simulations");

            int input=0;
            while (input == 0)
            {
                input = Console.ReadLine()[0] -'0';
            }

            switch (input)
            {
                case 1:
                    ShowPartyMenu(manager.allyParty);
                    break;
                case 2:
                    ShowPartyMenu(manager.enemyParty);
                    break;
                case 3:
                    SavePartyMenu(manager);
                    break;
                case 4:
                    LoadPartyMenu(manager);
                    break;
                case 5:
                    ChangeSimulationsCountMenu(simRunner);
                    break;
                case 6:
                    command = 2;
                    break;
            }
        }

        private void LoadPartyMenu(BattleManager manager)
        {
            Console.WriteLine("Do you want to load ally or enemy party? (ally/enemy)");
            string partyPick = "";
            while (partyPick == "")
            {
                partyPick = Console.ReadLine();
                if (partyPick != "ally" && partyPick != "enemy")
                {
                    Console.WriteLine("Wrong input!");
                    partyPick = "";
                }
            }

            Console.WriteLine("What name do you want to save your party as?");
            Console.WriteLine("Available saved parties:");
            List<string> files = (List<string>) Directory.EnumerateFiles("Setups", "*");
            foreach (string file in files)
            {
                Console.WriteLine(file);
            }
            string filename = "";
            while (filename == "")
            {
                filename = Console.ReadLine();
                if (!files.Contains(filename))
                {
                    Console.WriteLine("Choose a different name, ya smartass.");
                    filename = "";
                }
            }

            if (partyPick == "ally")
            {
                manager.allyParty = PartySerializer.Deserialize(String.Format("Setups/{0}",filename));
            }
            else
            {
                manager.enemyParty = PartySerializer.Deserialize(String.Format("Setups/{0}", filename));
            }
            Console.WriteLine("Succesfully loaded.");
        }

        private void SavePartyMenu(BattleManager manager)
        {
            Console.WriteLine("Do you want to save ally or enemy party? (ally/enemy)");
            string partyPick = "";
            while (partyPick == "")
            {
                partyPick = Console.ReadLine();
                if (partyPick != "ally" || partyPick != "enemy")
                {
                    Console.WriteLine("Wrong input!");
                    partyPick = "";
                }
                Console.WriteLine("Succesfully saved.");
            }

            Console.WriteLine("What name do you want to save your party as?");
            string filename = "";
            while (filename == "")
            {
                filename = Console.ReadLine();
                if (filename=="defaultAlly" || filename == "defaultEnemy")
                {
                    Console.WriteLine("Choose a different name, ya smartass.");
                    filename = "";
                }
            }

            Party party;
            if (partyPick == "ally") party = manager.allyParty;
            else party = manager.enemyParty;

            PartySerializer.Serialize(party, String.Format("Setups/{0}",filename));

        }
        private void ChangeSimulationsCountMenu(SimulationRunner simRunner)
        {
            Console.WriteLine("How many simulated combats do you want to run per session?");
            int simCount = 0;
            while (simCount == 0)
            {
                Console.ReadLine();
                int.TryParse("123", out simCount);
                if (simCount <= 0)
                {
                    Console.WriteLine("Wrong number!");
                    simCount = 0;
                }
            }
            Console.WriteLine("Succesfully changed.");
            simRunner.simCount = simCount;
        }
        public void ShowUpgradesMenu(Creature creature)
        {
            bool finished = false;
            while(!finished)
            {
                Console.Clear();
                ShowCreatureMenu(creature);
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
                Console.WriteLine("{0}: Go back", counter);



                    int input = Console.ReadLine()[0] - '0';
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
                        finished = true;
                    }
                
            }
            
        }
    }
}
