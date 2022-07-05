using DeepCrawlSims.AI;
using DeepCrawlSims.PartyNamespace;
using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DeepCrawlSims.BattleControl
{
    public struct Position
    {
        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }
        public override string ToString() => $"({X}, {Y})";
    }

    /// <summary>
    /// Main manager of the battle and moves within. 
    /// Controls the order of actions and assigns turns to creatures.
    /// </summary>
    [Serializable()]
    public class BattleManager
    {
        public Party allyParty;
        public Party enemyParty;
        public Creature currentCreature;
        private Controller controller; 
        public bool running = true;
        public BattleResults results;
        private int skipCounter;
        private bool artificialEnd = false;

        public BattleManager() { }

        public BattleManager(Party party, Party enemyParty)
        {
            this.allyParty = party;
            this.enemyParty = enemyParty;
            controller = new Controller();
            controller.manager = this;
            running = true;
        }


        public void RunOneTurn()
        {
            if (IsGameOver() || artificialEnd)
            {
                results = GenerateBattleResults();
                running = false;
                skipCounter = 0;
                artificialEnd = false;

            }
            else
            {
                NextCreaturesTurn();
                controller.CreatureActs(currentCreature);
            }
        }

        public void Reset()
        {            
            enemyParty.FullReset();
            allyParty.FullReset();
            running = true;
        }

        public void CurrentCreatureSkips()
        {
            skipCounter++;
            if (skipCounter > 5)
            {
                artificialEnd = true;
            }
        }

        /// <summary>
        /// Current creature performs a selected action towards target creature
        /// </summary>
        /// <param name="target">Picked target, ally or enemy</param>
        /// <param name="query">Picked or chosen action to perform</param>
        public void CurrentCreaturePlays(Creature target, Query query)
        {
            if (query.type == QueryType.None)
            {
                if (skipCounter > 5)
                {
                    artificialEnd = true;
                }
                skipCounter++;
            }
            else
            {
                skipCounter = 0;
            }

            if (query.type == QueryType.Attack || query.type == QueryType.AttackBuild)
            {
                // I want to play possible animations first because processing query through creatures might modify it
                query.type = QueryType.Animation;
                currentCreature.ProcessQuery(query);

                // And now i want the actually attack to proceed
                query.type = QueryType.Attack;
                target.ProcessQuery(query);
            }
        }

        /// <summary>
        /// Make a new creature take turn
        /// </summary>
        void NextCreaturesTurn()
        {
            int max_speed = 0;
            Creature nextCreature = null;
            bool found = false;

            var playerCharacters = allyParty.Creatures;
            var enemyCharacters = enemyParty.Creatures;

            while (!found)
            {
                // Search for a character with highest speed that havent moved this turn
                for (int i = 0; i < playerCharacters.Count; i++)
                {
                    var creature = playerCharacters[i];
                    int speed = creature.GetSpeed();
                    if (speed > max_speed)
                    {
                        max_speed = speed;
                        nextCreature = creature;
                    }
                }

                for (int i = 0; i < enemyCharacters.Count; i++)
                {
                    var creature = enemyCharacters[i];
                    int speed = creature.GetSpeed();
                    if (speed > max_speed)
                    {
                        max_speed = speed;
                        nextCreature = creature;
                    }
                }

                if (nextCreature == null)
                {
                    TriggerNewTurn();
                }
                else
                {
                    found = true;
                    currentCreature = nextCreature;
                    currentCreature.PayInitiativePoints();
                }
            }
        }

        private BattleResults GenerateBattleResults()
        {
            var retVal = new BattleResults();
            if (DidPartyLose(enemyParty))
                retVal.result = 1;
            else retVal.result = 2;

            retVal.allyHPs = new List<double>();
            retVal.enemyHPs = new List<double>();
            foreach (var item in allyParty.Creatures)
            {
                retVal.allyHPs.Add(item.GetHealth());
            }
            foreach (var item in enemyParty.Creatures)
            {
                retVal.enemyHPs.Add(item.GetHealth());
            }

            var allyMaxHPs = new List<double>();
            var enemyMaxHPs = new List<double>();

            foreach (var item in allyParty.Creatures)
            {
                allyMaxHPs.Add(item.GetMaxHealth());
            }
            foreach (var item in enemyParty.Creatures)
            {
                enemyMaxHPs.Add(item.GetMaxHealth());
            }

            Console.WriteLine(String.Format("Starting HP: ({0},{1},{2},{3}) vs. ({4},{5},{6},{7})",
                allyMaxHPs[0], allyMaxHPs[1], allyMaxHPs[2], allyMaxHPs[3],
                enemyMaxHPs[0], enemyMaxHPs[1], enemyMaxHPs[2], enemyMaxHPs[3]));

            Console.WriteLine(String.Format("Result HP: ({0},{1},{2},{3},) vs. ({4},{5},{6},{7})",
                retVal.allyHPs[0], retVal.allyHPs[1], retVal.allyHPs[2], retVal.allyHPs[3],
                retVal.enemyHPs[0], retVal.enemyHPs[1], retVal.enemyHPs[2], retVal.enemyHPs[3]));

            return retVal;
        }

        private bool IsGameOver()
        {
            return (DidPartyLose(allyParty) || DidPartyLose(enemyParty));
        }

        private bool DidPartyLose(Party party)
        {
            bool allDead = true;
            foreach (var item in party.Creatures)
            {
                var query = new Query(QueryType.Question);
                query.Add(QueryParameter.Dead, 0);
                item.ProcessQuery(query);
                if (query.parameters[QueryParameter.Dead] == 0)
                {
                    allDead = false;
                }
            }
            return allDead;
        }


        /// <summary>
        /// After all creatures have played, their speeds are refreshed and all timed status effects (like poison) trigger
        /// </summary>
        void TriggerNewTurn()
        {
            allyParty.TickTimedEffects();
            enemyParty.TickTimedEffects();
            enemyParty.ResetSpeed();
            allyParty.ResetSpeed();
        }

        public Creature GetCurrentCreature()
        {
            return currentCreature;
        }
    }
}