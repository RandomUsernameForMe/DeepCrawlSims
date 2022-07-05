using DeepCrawlSims.AI;
using DeepCrawlSims.BattleControl;
using DeepCrawlSims.PartyNamespace.Components;
using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCrawlSims.PartyNamespace
{

    /// <summary>
    /// Creature is anything that acts and makes decisions in combat.
    /// </summary>
    [Serializable()]
    public class Creature
    {
        public string name;
        public Controller controller;
        public bool isEnemy;
        public List<Component> components;

        public Creature(string name, bool isEnemy)
        {
            this.name = name;            
            this.isEnemy = isEnemy;
            controller = new Controller();
            components = new List<Component>();
        }

        /// <summary>
        /// Convenient way to ask if a creature is dead, sick, stuned or similar
        /// </summary>
        /// <param name="ind">The Status efect we are asking for</param>
        /// <returns>Answer</returns>
        public bool Is(QueryParameter ind)
        {
            var query = Query.question;
            query.Add(ind, 0);
            query = ProcessQuery(query);
            if (query.parameters[ind] == 0)
            {
                query.Clear();
                return false;
            }
            else
            {
                query.Clear();
                return true;
            }
        }       

        public int GetSpeed()
        {
            var speedComp = FetchComponent<Speed>();
            if (Is(QueryParameter.CanAct) && speedComp != null)
            {
                return speedComp.speed;
            }
            else return 0;
        }      


        /// <summary>
        /// Pass a Query object into all StatusEffcets in a creature. The effects are visited in topological order by their requirements. 
        /// </summary>
        /// <param name="query">Query carrying parameters and StatusBuilders</param>
        /// <returns>Query usually modified by status effects</returns>
        public Query ProcessQuery(Query query)
        {
            //Custom sorting mechanism
            components.Sort((a, b) => (TopologyWizard.GetInstance().Compare(a.GetType(), b.GetType())));


            for (int i = 0; i < components.Count; i++)
            {
                // each status effect may alter the processed query and that is expected behavior
                query = components[i].ProcessQuery(query);
            }

            // Query may carry StatusBuilders, which will created a new effect on hosting Creature (eg. poisoned)
            foreach (var item in query.effects.Values)
            {
                if (query.type == QueryType.Attack)
                {
                    //item.BuildStatusEffect(gameObject);
                    //components = new List<StatusEffect>(GetComponentsInChildren<StatusEffect>());
                }
            }
            return query;
        }


        internal T FetchComponent<T>() where T : Component
        {
            foreach (var item in components)
            {
                if (item is T) return item as T; 
            }
            return null;
        }
        internal List<T> FetchComponents<T>() where T : Component
        {
            var returnlist = new List<T>();
            foreach (var item in components)
            {
                if (item is T) returnlist.Add(item as T);
            }
            return returnlist;
        }

        void RemoveComponents<T>()
        {
            foreach (var item in components)
            {
                if (item is T) components.Remove(item);
            }            
        }

        public double GetHealth()
        {
            Health h = FetchComponent<Health>();
            if (h != null) return h.health;
            return 0;
            
        }
        public double GetMaxHealth()
        {
            Health health = FetchComponent<Health>();
            if (health != null) return health.maxHealth;
            return 0;
        }

        internal void FullHeal()
        {
            Health health = FetchComponent<Health>();
            if (health != null) health.health = health.maxHealth;
        }

        internal void ResetSpeed()
        {
            Speed speed = FetchComponent<Speed>();
            if (speed != null) speed.speed = speed.maxSpeed;
        }

        internal void FullReset()
        {
            FullHeal();
            ResetSpeed();
            RemoveComponents<TimedEffect>();
        }

        internal void PayInitiativePoints()
        {
            FetchComponent<Speed>().speed -= 4; //arbitrary; 
        }
    }

}