using DeepCrawlSims.PartyNamespace.Components;
using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public class LongWeapon : Component
{
    public override Query ProcessQuery(Query action)
    {
        if (action.type == QueryType.AttackBuilder)
        {
            if (action.parameters.ContainsKey(QueryParameter.Basic))
            {
                action.Add(QueryParameter.Enemy, 1);
            }
        }
        if (action.type == QueryType.Description)
        {
            if (action.parameters.ContainsKey(QueryParameter.Basic))
            {
                action.Add("Long Weapon strikes any enemy.");
            }
        }
        return action; 
    }
    public override List<(Type, Type)> GetRequirements()
    {
        var returnValue = new List<(Type, Type)>();
        returnValue.Add((typeof(Armor), typeof(Health)));
        return returnValue;
    }
}
