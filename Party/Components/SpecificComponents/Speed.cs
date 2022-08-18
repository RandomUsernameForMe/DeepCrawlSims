using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public class Speed : UpgradableComponent
{
    public int speed;
    public int maxSpeed;

    public override List<(Type, Type)> GetRequirements()
    {
        return null;
    }

    public Speed(int speed)
    {
        this.speed = speed;
        maxSpeed = speed;
    }

    public override Query ProcessQuery(Query query)
    {
        if (query.type == QueryType.Description)
        {
            if (query.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                query.Add(String.Format("Speed: {0}", maxSpeed));
            }
        }
        return query;
    }

    public override bool TryUpgrade(bool positive)
    {

        if (positive) speed += 1;
        else
        {
            speed -= 1;
            if (speed == 1) return false;
        }
        return true;
    }

    internal void ResetSpeed()
    {
        speed = maxSpeed;
    }

    internal void PayInitiativePoints()
    {
        speed -= 5;
    }
}
