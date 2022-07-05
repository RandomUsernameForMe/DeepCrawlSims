using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public class PowerStrike : UpgradableComponent
{
    public double power;

    public override Query ProcessQuery(Query action)
    {
        if (action.type == QueryType.AttackBuild)
        {
            if (action.parameters.ContainsKey(QueryParameter.Special))
            {
                action.Add(QueryParameter.Close, 1);
                action.Add(QueryParameter.Enemy, 1);
                action.Add(QueryParameter.PhysDmg, power);
            }            
        }
        if (action.type == QueryType.Description)
        {
            if (action.parameters.ContainsKey(QueryParameter.Special))
            {
                action.Add("Smash enemy with massive strike.");
            }
            if (action.parameters.ContainsKey(QueryParameter.SpecialName))
            {
                action.Add("PowerStrike");
            }
            if (action.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                action.Add(String.Format("PowerStrike: {0} dmg", power));
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
        if (power <= 20)
        {
            //Destroy(this);
            return true;
        }
        if (positive) power += 10;
        else power -= 10;
        return true;
    }
}
