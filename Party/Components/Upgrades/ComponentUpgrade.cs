using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCrawlSims.Party.Components.Upgrades
{
    class ComponentUpgrade<T> : Upgrade where T : UpgradableComponent
    {
        public override bool TryUpgrade(Creature creature, bool positive, bool unlimitedSpace)
        {
            var comp = creature.GetComponentInChildren<T>();
            if (comp == null)
            {
                var comps = creature.GetComponentsInChildren<UpgradableComponent>();
                if (comps.Length >= 6 && !unlimitedSpace) return false;

                creature.gameObject.AddComponent<T>();
                return true;
            }
            else
            {
                return comp.TryUpgrade(positive);
            }

        }

        public Type GetGenericType()
        {
            return typeof(T);
        }
    }
}
