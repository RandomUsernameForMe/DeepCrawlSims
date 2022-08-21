using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public class HealingWave : UpgradableComponent
{
    public int power = 20;
    public override Query ProcessQuery(Query action)
    {
        if (action.type == QueryType.AttackBuilder)
        {
            if (action.parameters.ContainsKey(QueryParameter.Special))
            {
                action.Add(QueryParameter.Healing, power);
            }
        }
        if (action.type == QueryType.Description)
        {
            if (action.parameters.ContainsKey(QueryParameter.SpecialName))
            {
                action.Add("Healing Touch");
            }
            if (action.parameters.ContainsKey(QueryParameter.Special))
            {
                action.Add("Restores health.");
            }
            if (action.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                action.Add(String.Format("Healing: {0} hp", power));
            }
        }
        return action;
    }

    public override List<(Type, Type)> GetRequirements()
    {
        var returnValue = new List<(Type, Type)>();
        returnValue.Add((typeof(HealingWave), typeof(Health)));
        return returnValue;
    }

    public override bool TryUpgrade(bool positive)
    {
        if (power < 20)
        {
            //Destroy(this);
            return true;
        }
        if (positive) power += 10;
        else power -= 10;
        return true;
    }
}
