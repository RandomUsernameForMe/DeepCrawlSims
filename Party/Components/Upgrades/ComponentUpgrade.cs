using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCrawlSims.PartyNamespace.Components.Upgrades
{
    class ComponentUpgrade<T> : Upgrade where T : UpgradableComponent, new()
    {
        public override void UpgradeOrAdd(Creature creature, bool positive, bool unlimitedSpace) 
        {
            bool found = false;
            foreach (var item in creature.components)
            {
                if (item is T)
                {
                    (item as UpgradableComponent).TryUpgrade(true);
                    found = true; ;
                }
            }
            if (!found)
            {
                creature.components.Add(new T());
            }            
        }
    }
}
