using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCrawlSims.PartyNamespace.Components.Upgrades
{
    class ComponentUpgrade<T> : Upgrade where T : UpgradableComponent
    {
        public override bool TryUpgrade(Creature creature, bool positive, bool unlimitedSpace) //TODO
        {
            foreach (var item in creature.components)
            {
                if (item is T)
                {
                    (item as UpgradableComponent).TryUpgrade(true);
                    return true;
                }
            }
            return false;
        }

        public Type GetGenericType()
        {
            return typeof(T);
        }
    }
}
