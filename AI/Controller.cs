using DeepCrawlSims.BattleControl;
using DeepCrawlSims.Party;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCrawlSims.AI
{


    public class Controller
    {
        public BattleManager manager;

        /// <summary>
        /// General function to handle everything when computer wants to decide what to play
        /// </summary>
        /// <param name="creature"></param>
        public void CreatureActs(Creature creature)
        {
            // theres now a 250% chance of a calculated attack
            int rnd = Random.Range(0, 2); //TODO
            Query query = null; ;
            Creature target = null; ;

            if (rnd == 1)
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
                    query = new Query(QueryType.AttackBuild);
                    if (t.Item1 == 0) query.Add(QueryParameter.Basic, 0);
                    else query.Add(QueryParameter.Special, 0);
                    query = creature.GetComponent<QueryHandler>().ProcessQuery(query);

                    // Prepare target 
                    target = TargetingSystem.GetCreatureByPosition(t.Item2, manager);
                }
            }
            else
            {
                var found = false;
                for (int i = 0; i < 3; i++)
                {
                    if (!found)
                    {
                        rnd = UnityEngine.Random.Range(0, 2);
                        query = new Query(QueryType.AttackBuild);
                        if (rnd == 0) query.Add(QueryParameter.Basic, 0);
                        else query.Add(QueryParameter.Special, 0);
                        query = creature.GetComponent<QueryHandler>().ProcessQuery(query);
                        found = ActionHasViableTargets(query, creature);
                    }
                }

                if (found) target = PickRandomTarget(query, creature);
                else query.type = QueryType.None;
            }
            manager.CurrentCreaturePlays(target, query);
        }


        public (int, int) PickActionToPlay(Creature creature)
        {
            Query query;

            var betterAI = true;
            if (betterAI)
            {
                var results = new Dictionary<(int, int), double>();

                query = new Query(QueryType.AttackBuild);
                query.Add(QueryParameter.Basic, 0);
                query = creature.GetComponent<QueryHandler>().ProcessQuery(query);
                List<QueryParameter> keys = new List<QueryParameter>(query.parameters.Keys);
                List<int> pos = TargetingSystem.PickViableTargets(keys, creature.isEnemy);

                TryAllPossibleTargets(query, pos, results, 0);

                query = new Query(QueryType.AttackBuild);
                query.Add(QueryParameter.Special, 0);
                query = creature.GetComponent<QueryHandler>().ProcessQuery(query);
                keys = new List<QueryParameter>(query.parameters.Keys);
                pos = TargetingSystem.PickViableTargets(keys, creature.isEnemy);

                TryAllPossibleTargets(query, pos, results, 1);

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
            return (0, 0);
        }

        private void TryAllPossibleTargets(Query origQuery, List<int> pos, Dictionary<(int, int), double> results, int i)
        {
            foreach (var item in pos)
            {
                Query query = new Query(origQuery);
                Creature cre = TargetingSystem.GetCreatureByPosition(item, manager);
                query.type = QueryType.Question;
                query = cre.GetComponent<QueryHandler>().ProcessQuery(query);
                results.Add((i, item), query.parameters[QueryParameter.CalcultedDmg]);
            }
        }

        private bool ActionHasViableTargets(Query query, Creature creature)
        {
            List<QueryParameter> keys = new List<QueryParameter>(query.parameters.Keys);
            List<int> pos = TargetingSystem.PickViableTargets(keys, creature.isEnemy);

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
            List<int> pos = TargetingSystem.PickViableTargets(keys, creature.isEnemy);
            var targetFound = false;
            Creature cre = null;


            if (!ActionHasViableTargets(query, creature))
            {
                Debug.Log("problemo");
            }

            //at the moment its random
            while (!targetFound)
            {
                int rnd = UnityEngine.Random.Range(0, pos.Count);
                cre = TargetingSystem.GetCreatureByPosition(pos[rnd], manager);
                if (!cre.Is(QueryParameter.Dead)) targetFound = true;
            }
            return cre;
        }
    }
}
