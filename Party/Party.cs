using DeepCrawlSims.BattleControl;
using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCrawlSims.PartyNamespace
{

    /// <summary>
    /// Structure holding and managing a group of creatures, ally or foe.
    /// Ally party is transfered in between senes, Enemy party is generated in each battle.
    /// </summary>
    [Serializable()]
    public class Party 
    {
        private List<Creature> creatures;

        public Party()
        {
            creatures = new List<Creature>();
        }

        public List<Creature> Creatures { get => creatures; set => creatures = value; }

        /// <summary>
        /// Triggers timed effects such as poison or stun.
        /// </summary>
        public void TickTimedEffects()
        {
            for (int i = 0; i < Creatures.Count; i++)
            {
                
                Creature creature = Creatures[i];
                var effects = creature.FetchComponents<TimedEffect>();

                for (int j = 0; j < effects.Count; j++)
                {
                    var item = effects[j];
                    if (item.active)
                    {
                        Query action = item.Tick();
                        object p = creature.ProcessQuery(action);
                        if (item.timer <= 0)
                        {
                            creature.components.Remove(item);
                        }
                    }
                }
            }
        }

        internal void FullReset()
        {
            foreach (var item in Creatures)
            {
                item.FullReset(); ;
            }
        }

        public void ResetSpeed()
        {
            for (int i = 0; i < Creatures.Count; i++)
            {
                Creatures[i].ResetSpeed();
            }
        }

        public override string ToString()
        {
            var HPs = new List<double>();
            foreach (var item in Creatures)
            {
                HPs.Add(item.GetHealth());
            }

            var names = new List<string>();
            foreach (var item in Creatures)
            {
                names.Add(item.name);
            }

            return String.Format("({4}: {0}, {5}: {1}, {6}: {2}, {7}: {3})",
                HPs[0], HPs[1], HPs[2], HPs[3],
               names[0], names[1], names[2], names[3]);
        }
    }

}
