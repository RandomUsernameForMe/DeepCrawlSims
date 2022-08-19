using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public class ElementalResistance : UpgradableComponent
{
    public double resistance;

    public override Query ProcessQuery(Query action)
    {
        if (action.type == QueryType.Attack)
        {
            if (action.effects.ContainsKey(QueryParameter.Poison))
            {
                (action.effects[QueryParameter.Poison]as Poison).potency = Math.Floor((action.effects[QueryParameter.Poison] as Poison).potency * (1-resistance));
            }
            if (action.effects.ContainsKey(QueryParameter.FireDmg))
            {
                action.parameters[QueryParameter.FireDmg] = Math.Floor(action.parameters[QueryParameter.FireDmg] * (1 - resistance));
            }
        }
        if (action.type == QueryType.Description)
        {
            if (action.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                action.Add(String.Format("Poison resist: {0} %", resistance*100));
            }
        }
        return action;
    }

    public override List<(Type, Type)> GetRequirements()
    {
        var returnValue = new List<(Type, Type)>();
        returnValue.Add((typeof(ElementalResistance), typeof(Health)));
        return returnValue;
    }

    public override bool TryUpgrade(bool positive)
    {
        if (positive) resistance += 0.2;
        else
        {
            if (resistance <= 0.2) return false;
            resistance -= 0.2;
        }
        return true;
    }
}
