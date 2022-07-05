using DeepCrawlSims.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCrawlSims.Party
{

    /// <summary>
    /// Creature is anything that acts and makes decisions in combat.
    /// </summary>
    public class Creature
    {
        public string name;
        public Controller controller;
        public bool isEnemy;
        public bool isAI;
        public List<Component> components;

        /// <summary>
        /// In case creature changes its controls (eg. mind control spell), we need to refresh controller every time its about to act
        /// </summary>
        public void DecideYourMove()
        {
            controller.CreatureActs(this);
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

        public void ResetSpeed()
        {
            GetComponentInChildren<Speed>().ResetSpeed();

        }

        public int GetSpeed()
        {
            var speedComp = GetComponentInChildren<Speed>();
            if (Is(QueryParameter.CanAct) && speedComp != null)
            {
                return speedComp.speed;
            }
            else return 0;
        }

        public void UpdateUI()
        {
            GetComponentInChildren<CreatureStatsDescriptionPanel>().UpdateUI();
        }

        public void Highlight()
        {
            GetComponentInChildren<RingHighlighter>().SetToHighlightColor();
        }

        internal Query ProcessQuery(Query action)
        {
            return GetComponent<QueryHandler>().ProcessQuery(action);
        }

        public double GetHealth()
        {
            Health h = GetComponentInChildren<Health>();
            if (h != null)
            {
                return h.health;
            }
            else
            {
                return 0;
            }
        }
        public double GetMaxHealth()
        {
            Health health = GetComponentInChildren<Health>();
            if (health != null) return health.maxHealth;
            return 0;
        }

        public void FullHeal()
        {
            Health health = GetComponentInChildren<Health>();
            health.health = health.maxHealth;
        }

        public void Move(Vector3 other)
        {
            transform.parent.position = other;
        }

        internal void FullReset()
        {
            FullHeal();
            var tempEffects = GetComponentsInChildren<TimedEffect>();
            foreach (var item in tempEffects)
            {
                Destroy(item);
            }
        }

        internal void PayInitiativePoints()
        {
            GetComponentInChildren<Speed>().PayInitiativePoints();
        }
    }

}