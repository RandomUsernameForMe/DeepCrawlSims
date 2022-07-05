using DeepCrawlSims.BattleControl;
using DeepCrawlSims.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCrawlSims.Party
{

    /// <summary>
    /// Structure holding and managing a group of creatures, ally or foe.
    /// Ally party is transfered in between senes, Enemy party is generated in each battle.
    /// </summary>
    public class Party 
    {
        public List<Creature> creatures;

        /// <summary>
        /// Triggers timed effects such as poison or stun.
        /// </summary>
        public void TickTimedEffects()
        {
            for (int i = 0; i < creatures.Count; i++)
            {
                var effects = creatures[i].GetComponentsInChildren<TimedEffect>();
                var partyCreatures = GetParty();
                Creature creature = partyCreatures[i];

                for (int j = 0; j < effects.Length; j++)
                {
                    var item = effects[j];
                    if (item.active)
                    {
                        MyQuery action = item.Tick();
                        creature.ProcessQuery(action);
                        if (item.timer <= 0)
                        {
                            Destroy(item);
                            item.active = false;
                        }
                    }
                }
            }
        }

        internal void FullReset()
        {
            foreach (var item in creatures)
            {
                item.FullReset(); ;
            }
        }

        public void ResetSpeed()
        {

            for (int i = 0; i < creatures.Count; i++)
            {
                creatures[i].ResetSpeed();
            }
        }

        public override string ToString()
        {
            var HPs = new List<double>();
            foreach (var item in creatures)
            {
                HPs.Add(item.GetMaxHealth());
            }

            var names = new List<string>();
            foreach (var item in creatures)
            {
                names.Add(item.name);
            }

            return String.Format("({4}: {0}, {5}: {1}, {6}: {2}, {7}: {3})",
                HPs[0], HPs[1], HPs[2], HPs[3],
               names[0], names[1], names[2], names[3]);
        }
    }

}
