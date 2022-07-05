using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCrawlSims.Party.Components
{

    public abstract class Component
    {
        abstract public List<(Type, Type)> GetRequirements();

        abstract public Query ProcessQuery(Query query);
    }

    public abstract class ValueComponent : Component
    {
        public int value;
        public void Upgrade(int val)
        {
            value += val;
        }
    }
}
