﻿using DeepCrawlSims.AI;
using DeepCrawlSims.PartyNamespace;
using DeepCrawlSims.QueryNamespace;
using DeepCrawlSims.Simulations;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool isRunning = true;
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
            isRunning = true;
        }

        public void RunOneTurn()
        {
            if (IsGameOver() || artificialEnd)
            {
                results = GenerateBattleResults();
                isRunning = false;
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
            isRunning = true;
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

            if (query.type == QueryType.Attack || query.type == QueryType.AttackBuilder)
            {
                // mostly for debugging purposes
                if (Global.verbose) if (query.parameters.ContainsKey(QueryParameter.Basic)) 
                        Console.WriteLine(String.Format("{0} performs a basic attack targetting {1}.", currentCreature.GetNameWithAlegience(), target.GetNameWithAlegience()));
                else Console.WriteLine(String.Format("{0} performs its special ability targetting {1}.", currentCreature.GetNameWithAlegience(),target.GetNameWithAlegience()));

                // And now i want the actually attack to proceed
                query.type = QueryType.Attack;
                target.ProcessQuery(query);
            }
        }

        /// <summary>
        /// Assign a new creature to take turn 
        /// </summary>
        void NextCreaturesTurn()
        {
            int max_speed = 0;
            Creature nextCreature = null;
            bool found = false;

            var playerCharacters = allyParty.Creatures;
            var enemyCharacters = enemyParty.Creatures;

            var rnd = new Random();
            var allCharacters = playerCharacters.Concat(enemyCharacters).ToList().OrderBy(item => rnd.Next());

            while (!found)
            {
                foreach (var item in allCharacters)
                {
                    int speed = item.GetSpeed();
                    if (speed > max_speed)
                    {
                        max_speed = speed;
                        nextCreature = item;
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
            if (Global.verbose)
            {
                Console.WriteLine(String.Format("Starting HP: ({0},{1},{2},{3}) vs. ({4},{5},{6},{7})",
                    allyMaxHPs[0], allyMaxHPs[1], allyMaxHPs[2], allyMaxHPs[3],
                    enemyMaxHPs[0], enemyMaxHPs[1], enemyMaxHPs[2], enemyMaxHPs[3]));

                Console.WriteLine(String.Format("Result HP: ({0},{1},{2},{3},) vs. ({4},{5},{6},{7})",
                    retVal.allyHPs[0], retVal.allyHPs[1], retVal.allyHPs[2], retVal.allyHPs[3],
                    retVal.enemyHPs[0], retVal.enemyHPs[1], retVal.enemyHPs[2], retVal.enemyHPs[3]));

            }
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