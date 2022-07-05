using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCrawlSims.PartyNamespace.Components.Upgrades
{
    class ComponentUpgrade<T> : Upgrade where T : UpgradableComponent
    {
        public override bool TryUpgrade(Creature creature, bool positive, bool unlimitedSpace) //TODO
        {         
            
            
                return true;
            

        }

        public Type GetGenericType()
        {
            return typeof(T);
        }
    }
}
