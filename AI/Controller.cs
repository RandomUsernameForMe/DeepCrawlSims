using DeepCrawlSims.BattleControl;
using DeepCrawlSims.PartyNamespace;
using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCrawlSims.AI
{

    [Serializable()]
    public class Controller
    {
        public BattleManager manager;
        static int BASIC_ATTACK_CONSTANT = 0;
        static int SPECIAL_CONSTANT = 1;

        /// <summary>
        /// General function to handle everything when controller wants to decide what to play
        /// </summary>
        /// <param name="creature"></param>
        public void CreatureActs(Creature creature)
        {
            // theres now a 50% chance of a calculated attack
            Random rnd = new Random();
            int rndInt = rnd.Next(0, 2);
            Query query = null; ;
            Creature target = null; ;

            // when picking a calculated action
            if (rndInt == 1)
            {
                // Choose attack 
                (int, int) t = PickActionToPlay(creature);

                if (t == (-1, -1))
                {
                    query = new Query(QueryType.None);
                    target = null;
                }
                else
                {
                    // Prepare attack 
                    query = new Query(QueryType.AttackBuilder);
                    if (t.Item1 == BASIC_ATTACK_CONSTANT) query.Add(QueryParameter.Basic, 0);
                    else query.Add(QueryParameter.Special, 0);
                    query = creature.ProcessQuery(query);

                    // Prepare target 
                    target = TargetingSystem.GetCreatureByPosition(t.Item2, manager);
                }
            }

            // when picking a random action
            else
            {
                var found = false;
                for (int i = 0; i < 3; i++)
                {
                    if (!found)
                    {
                        rndInt = rnd.Next(0, 2);
                        query = new Query(QueryType.AttackBuilder);
                        if (rndInt == BASIC_ATTACK_CONSTANT) query.Add(QueryParameter.Basic, 0);
                        else query.Add(QueryParameter.Special, 0);
                        query = creature.ProcessQuery(query);
                        found = DoesActionHaveViableTargets(query, creature);
                    }
                }
                if (found) target = PickRandomTarget(query, creature);
                else query.type = QueryType.None;
            }
            manager.CurrentCreaturePlays(target, query);
        }

        /// <summary>
        /// Picks the action and target amounting to the maximum possible damage done
        /// </summary>
        /// <param name="creature"></param>
        /// <returns>tuple, first int describing type of attack, second int number of targeted creature</returns>
        public (int, int) PickActionToPlay(Creature creature)
        {
            Query query;
            var results = new Dictionary<(int, int), double>();

            // Calculate damage a basic attack would do to its possible targets

            query = new Query(QueryType.AttackBuilder);
            query.Add(QueryParameter.Basic, 0);
            query = creature.ProcessQuery(query);
            List<QueryParameter> keys = new List<QueryParameter>(query.parameters.Keys);
            List<int> pos = TargetingSystem.ListViableTargets(keys, creature.isOnOpposingSide);
            TryAllPossibleTargets(query, pos, results, BASIC_ATTACK_CONSTANT);

            // Calculate damage a special ability would do to its possible targets
            // (Works with heling, but doesnt calculate poison damage) 

            query = new Query(QueryType.AttackBuilder);
            query.Add(QueryParameter.Special, 0);
            query = creature.ProcessQuery(query);
            keys = new List<QueryParameter>(query.parameters.Keys);
            pos = TargetingSystem.ListViableTargets(keys, creature.isOnOpposingSide);
            TryAllPossibleTargets(query, pos, results, SPECIAL_CONSTANT);

            // pick the ability that doesnt the most possible damage

            double max = 0;
            (int, int) maxItem = (-1, -1);
            foreach (var item in results.Keys)
            {
                if (results[item] > max && !TargetingSystem.GetCreatureByPosition(item.Item2, manager).Is(QueryParameter.Dead))
                {
                    max = results[item];
                    maxItem = item;
                }
            }
            return (maxItem.Item1, maxItem.Item2);
        }

        private void TryAllPossibleTargets(Query origQuery, List<int> pos, Dictionary<(int, int), double> results, int i)
        {
            foreach (var item in pos)
            {
                Query query = new Query(origQuery);
                Creature cre = TargetingSystem.GetCreatureByPosition(item, manager);
                query.type = QueryType.Question;
                query = cre.ProcessQuery(query);
                results.Add((i, item), query.parameters[QueryParameter.CalcultedDmg]);
            }
        }

        private bool DoesActionHaveViableTargets(Query query, Creature creature)
        {
            List<QueryParameter> keys = new List<QueryParameter>(query.parameters.Keys);
            List<int> pos = TargetingSystem.ListViableTargets(keys, creature.isOnOpposingSide);
            bool viableTargetFound = false;

            foreach (var item in pos)
            {
                Creature cre = TargetingSystem.GetCreatureByPosition(item, manager);
                if (!cre.Is(QueryParameter.Dead)) viableTargetFound = true;
            }
            return viableTargetFound;
        }

        public Creature PickRandomTarget(Query query, Creature creature)
        {
            List<QueryParameter> keys = new List<QueryParameter>(query.parameters.Keys);
            List<int> pos = TargetingSystem.ListViableTargets(keys, creature.isOnOpposingSide);
            var targetFound = false;
            Creature cre = null;

            while (!targetFound)
            {
                Random rnd = new Random();
                int rndInt = rnd.Next(0, pos.Count);
                cre = TargetingSystem.GetCreatureByPosition(pos[rndInt], manager);
                if (!cre.Is(QueryParameter.Dead)) targetFound = true;
            }
            return cre;
        }
    }
}
