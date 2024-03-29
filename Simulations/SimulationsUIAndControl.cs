﻿using DeepCrawlSims.BattleControl;
using DeepCrawlSims.PartyNamespace;
using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeepCrawlSims.Simulations
{
    public static class Global
    {
        public static bool verbose = false;
        public static bool multithread = false;
    }
    /// <summary>
    /// All the UI user interacts with using the program. Its a pseudostate machine switching between states of the menu 
    /// </summary>
    class SimulationsUIAndControl
    {
        const int RUN_CONST = 2;
        const int MENU_CONST = 1;

        BattleManager manager;
        SimulationRunner simRunner;
        int command = MENU_CONST;

        public SimulationsUIAndControl(BattleManager battleManager)
        {
            manager = battleManager;
        }

        public void RunUI()
        {
            simRunner = new SimulationRunner(manager, 100);
            while (command != 0)
            {
                switch (command)
                {
                    case MENU_CONST:
                        ShowMainMenu();
                        break;
                    case RUN_CONST:
                        Console.WriteLine("...");
                        Console.WriteLine("Running simulations...");
                        simRunner.Run();
                        Console.WriteLine($"Press Enter to continue");
                        Console.ReadLine();
                        command = MENU_CONST;
                        break;                   
                }
            }
        }

        /// <summary>
        /// Menu showing all creatures in a party and giving the option to pick one to modify.
        /// </summary>
        public void ShowPartyMenu(Party party)
        {
            bool finished = false;
            while (!finished) {
                Console.Clear();
                foreach (var creature in party.Creatures)
                {
                    ShowCreature(creature);
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

        /// <summary>
        /// UI item describing components of one creature
        /// </summary>
        /// <param name="creature"></param>
        public void ShowCreature(Creature creature)
        {
            Console.WriteLine("Name: {0}", creature.name);
            Query query = new Query(QueryType.Description);
            query.Add(QueryParameter.Tooltip, 0);
            string description = string.Join("\n", creature.ProcessQuery(query).descs);
            Console.WriteLine(description);
            Console.WriteLine();
        }

        /// <summary>
        /// Main menu, main branching state for user controling the program  
        /// </summary>
        public void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("Verbose mode is turned " + (Global.verbose ? "ON" : "OFF"));
            Console.WriteLine("Multithreading mode is turned " + (Global.multithread ? "ON" : "OFF"));
            Console.WriteLine("Current number of simulations to run: " + simRunner.simCount);
            Console.WriteLine();
            Console.WriteLine("What do you want to do now? (enter apropriate number)");
            Console.WriteLine("1 = Show/modify first party");
            Console.WriteLine("2 = Show/show second party");
            Console.WriteLine("3 = Save setup to file");
            Console.WriteLine("4 = Load setup from file");
            Console.WriteLine("5 = Modify number of simulations");
            Console.WriteLine("6 = Switch verbose mode ON/OFF");
            Console.WriteLine("7 = Switch multithreading mode ON/OFF");
            Console.WriteLine("8 = Run simulations");

            int input=0;
            while (input == 0)
            {
                string inp = Console.ReadLine(); 
                if (inp.Length>0) input = inp[0] -'0';
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
                    Global.verbose = !Global.verbose;
                    break;
                case 7:
                    Global.multithread = !Global.multithread;
                    break;
                case 8:
                    command = 2;
                    break;
            }
        }

        /// <summary>
        /// Sub-menu for when user wants to load party setup from a file.
        /// </summary>
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
            var files = Directory.EnumerateFiles("Setups", "*");
            foreach (string file in files)
            {                
                Console.WriteLine(file.Remove(0, 7));
            }
            string filename = "";
            while (filename == "")
            {
                filename = "Setups\\"+ Console.ReadLine();
                bool found = false;
                foreach (var item in files)
                {
                    if (item == filename) found = true; 
                }
                if (!found)
                {
                    Console.WriteLine("Choose a different name, ya smartass.");
                    filename = "";
                }
            }

            try
            {
                if (partyPick == "ally")
                {
                    manager.allyParty = PartySerializer.Deserialize(String.Format("{0}", filename));
                    foreach (var item in manager.enemyParty.Creatures)
                    {
                        item.isOnOpposingSide = false;
                    }
                }
                else
                {
                    manager.enemyParty = PartySerializer.Deserialize(String.Format("{0}", filename));
                    foreach (var item in manager.enemyParty.Creatures)
                    {
                        item.isOnOpposingSide = true;
                    }
                }
                Console.WriteLine("Succesfully loaded.");
            }
            catch
            {
                Console.WriteLine("Unsuccesfully loaded. Something went wrong.");
                Console.WriteLine("Press Enter to continue");
                Console.ReadLine();
            }

        }

        /// <summary>
        /// Sub-menu for when user wants to save party setup into a file
        /// </summary>
        private void SavePartyMenu(BattleManager manager)
        {
            Console.WriteLine("Do you want to save ally or enemy party? (ally/enemy)");
            string partyPick = "";
            while (partyPick == "")
            {
                partyPick = Console.ReadLine();
                if (partyPick != "ally" && partyPick != "enemy")
                {
                    Console.WriteLine("Wrong input!");
                    partyPick = "";
                }
                else
                {
                    Console.WriteLine("Succesfully saved.");
                }                
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
        
        /// <summary>
        /// Sub-menu for when user wants to change the number of simulations ran
        /// </summary>
        private void ChangeSimulationsCountMenu(SimulationRunner simRunner)
        {
            Console.WriteLine("How many simulated combats do you want to run per session?");
            int simCount = 0;
            while (simCount == 0)
            {
                
                int.TryParse(Console.ReadLine(), out simCount);
                if (simCount <= 0)
                {
                    Console.WriteLine("Wrong number!");
                    simCount = 0;
                }
            }
            Console.WriteLine("Succesfully changed.");
            simRunner.simCount = simCount;
        }

        /// <summary>
        /// Show menu for modifications for a specific creature
        /// </summary>
        /// <param name="creature">creature to be modified</param>
        public void ShowUpgradesMenu(Creature creature)
        {
            bool finished = false;
            while(!finished)
            {
                Console.Clear();
                ShowCreature(creature);
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
                        passed[input - 1].ApplyUpgrade(creature, true, true);
                        Console.WriteLine("Upgrade Succesful!");
                        
                    }
                    if (input == counter)
                    {
                        finished = true;
                    }
                
            }
            
        }
    }
}
