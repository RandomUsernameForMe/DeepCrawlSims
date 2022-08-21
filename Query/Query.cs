using DeepCrawlSims.PartyNamespace.Components;
using System.Collections.Generic;

namespace DeepCrawlSims.QueryNamespace
{
    public enum QueryType
    {
        Attack,
        Description,
        AttackBuilder,
        Question,
        Tick,
        None,
    }
    public enum QueryParameter
    {
        PhysDmg,
        MagicDmg,
        TrueDmg,
        PercentDmg,
        CalcultedDmg,
        PoisonAmp,
        Poison,
        Healing,
        Stun,
        Move,
        Claws,

        // Question 
        CanAct,
        Dead,
        DestroyerUsed,

        // Description
        SpecialName,
        Basic,
        Special,
        Description,
        Tooltip,

        // Targeting
        Close,
        Far,
        Enemy,
        Ally,
        FireDmg,
        Executioner,
    }
    public class Query
    {
        public QueryType type;
        public Dictionary<QueryParameter, double> parameters;
        public List<string> descs = new List<string>();
        public Dictionary<QueryParameter, Component> effects = new Dictionary<QueryParameter, Component>();

        public static Query question = new Query(QueryType.Question);

        public Query(QueryType type)
        {
            this.type = type;
            parameters = new Dictionary<QueryParameter, double>();
        }

        public Query(Query other)
        {
            this.type = other.type;
            parameters = new Dictionary<QueryParameter, double>(other.parameters);
            effects = new Dictionary<QueryParameter, Component>(other.effects);
            descs = new List<string>(other.descs);
        }

        public void Clear()
        {
            parameters.Clear();
            descs.Clear();
            effects.Clear();
        }

        public void Add(QueryParameter str, double val)
        {
            if (parameters.ContainsKey(str))
            {
                parameters[str] += val;
            }
            else
            {
                parameters.Add(str, val);
            }
        }

        public void Add(string str)
        {
            if (!descs.Contains(str))
            {
                descs.Add(str);
            }
        }

        public void Add(QueryParameter ind, Component eff)
        {
            if (!effects.ContainsKey(ind))
            {
                effects.Add(ind, eff);
            }
        }

    }

}