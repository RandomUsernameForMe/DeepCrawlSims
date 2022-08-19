using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public class FieryWeapons : UpgradableComponent
{
    public double power = 5;
    public override Query ProcessQuery(Query action)
    {
        if (action.type == QueryType.AttackBuild)
        {
            if (action.parameters.ContainsKey(QueryParameter.Special))
            {
                action.Add(QueryParameter.FireDmg, power);
            }
            if (action.parameters.ContainsKey(QueryParameter.Basic))
            {
                action.Add(QueryParameter.FireDmg, power);
            }
        }
        if (action.type == QueryType.Description)
        {
            if (action.parameters.ContainsKey(QueryParameter.Special))
            {
                action.Add("Deals bonus fire damage.");
            }
            if (action.parameters.ContainsKey(QueryParameter.Basic))
            {
                action.Add("Deals bonus fire damage.");
            }

            if (action.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                action.Add(String.Format("All abilities deal bonus {0} fire dmg",power));
            }
        }

        return action;
    }
    public override List<(Type, Type)> GetRequirements()
    {
        return null;
    }

    public override bool TryUpgrade(bool positive)
    {

        if (positive) power += 5;
        else power -= 5;
        return true;
    }
}
