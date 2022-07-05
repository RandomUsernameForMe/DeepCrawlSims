using System;
using System.Collections.Generic;
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
    public class BattleManager
    {
        public Party allyParty;
        public Party enemyParty;
        public Creature currentCreature;
        public bool loaded = false;
        public bool running = false;
        public BattleResults results;
        public float minimalTimeBetweenMoves;
        private int skipCounter;
        private bool artificialEnd = false;

        

        public delegate void OnCharacterPlayedEvent();
        public static event OnCharacterPlayedEvent OnCharacterFinishedTurn;



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
                currentCreature.DecideYourMove();
            }
        }

        public void LoadAndReset()
        {
            if (allyParty == null) allyParty = GameObject.Find("AllyParty").GetComponent<Party>();
            if (enemyParty == null) enemyParty = GameObject.Find("EnemyParty").GetComponent<Party>();
            var spriteGenerator = GetComponent<SpriteGenerator>();
            if (spriteGenerator != null) spriteGenerator.AssignSpritesToEnemyParty();
            results = null;
            enemyParty.FullReset();
            allyParty.FullReset();
            running = true;
            OnBattleLoaded();
        }

        public void CurrentCreaturSkips()
        {
            skipCounter++;
            if (skipCounter > 5)
            {
                artificialEnd = true;
            }
            OnCharacterFinishedTurn();
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

            if (query.type == QueryType.Swap)
            {
                Vector3 buffer = currentCreature.transform.position;
                currentCreature.Move(target.transform.position);
                target.Move(buffer);
                target.UpdateUI();
            }

            if (query.type == QueryType.Attack || query.type == QueryType.AttackBuild)
            {
                // I want to play possible animations first because processing query through creatures might modify it
                query.type = QueryType.Animation;
                currentCreature.GetComponent<QueryHandler>().ProcessQuery(query);

                // And now i want the actually attack to proceed
                query.type = QueryType.Attack;
                target.GetComponent<QueryHandler>().ProcessQuery(query);
                target.UpdateUI();
            }
            OnCharacterFinishedTurn();
        }

        /// <summary>
        /// Make a new creature take turn
        /// </summary>
        void NextCreaturesTurn()
        {
            int max_speed = 0;
            Creature nextCreature = null;
            bool found = false;

            var playerCharacters = allyParty.GetParty();
            var enemyCharacters = enemyParty.GetParty();

            while (!found)
            {
                // Search for a character with highest speed that havent moved this turn
                for (int i = 0; i < playerCharacters.Count; i++)
                {
                    var creature = playerCharacters[i].GetComponent<Creature>();
                    int speed = creature.GetSpeed();
                    if (speed > max_speed)
                    {
                        max_speed = speed;
                        nextCreature = creature;
                    }
                }

                for (int i = 0; i < enemyCharacters.Count; i++)
                {
                    var creature = enemyCharacters[i].GetComponent<Creature>();
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
            if (BattleTransitions.DidPartyLose(enemyParty))
                retVal.result = 1;
            else retVal.result = 2;

            retVal.allyHPs = new List<double>();
            retVal.enemyHPs = new List<double>();
            foreach (var item in allyParty.party)
            {
                retVal.allyHPs.Add(item.GetComponentInChildren<Creature>().GetHealth());
            }
            foreach (var item in enemyParty.party)
            {
                retVal.enemyHPs.Add(item.GetComponentInChildren<Creature>().GetHealth());
            }

            var allyMaxHPs = new List<double>();
            var enemyMaxHPs = new List<double>();

            foreach (var item in allyParty.party)
            {
                allyMaxHPs.Add(item.GetComponentInChildren<Creature>().GetMaxHealth());
            }
            foreach (var item in enemyParty.party)
            {
                enemyMaxHPs.Add(item.GetComponentInChildren<Creature>().GetMaxHealth());
            }

            Debug.Log(String.Format("Starting HP: ({0},{1},{2},{3}) vs. ({4},{5},{6},{7})",
                allyMaxHPs[0], allyMaxHPs[1], allyMaxHPs[2], allyMaxHPs[3],
                enemyMaxHPs[0], enemyMaxHPs[1], enemyMaxHPs[2], enemyMaxHPs[3]));

            //Debug.Log(String.Format("Result HP: ({0},{1},{2},{3},) vs. ({4},{5},{6},{7})",
            //    retVal.allyHPs[0], retVal.allyHPs[1], retVal.allyHPs[2], retVal.allyHPs[3],
            //    retVal.enemyHPs[0], retVal.enemyHPs[1], retVal.enemyHPs[2], retVal.enemyHPs[3]));

            return retVal;
        }

        private bool IsGameOver()
        {
            return (BattleTransitions.DidPartyLose(allyParty) || BattleTransitions.DidPartyLose(enemyParty));
        }

        /// <summary>
        /// After all creatures have played, their speeds are refreshed and all timed status effects (like poison) trigger
        /// </summary>
        void TriggerNewTurn()
        {
            allyParty.TickTimedEffects();
            enemyParty.TickTimedEffects();
            OnRoundEnded();
        }

        public Creature GetCurrentCreature()
        {
            return currentCreature;
        }
    }
}