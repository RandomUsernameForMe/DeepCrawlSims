using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public class Stun : TimedEffect
{
    public Stun(int duration)
    {
        this.timer = duration;
    }
    public override List<(Type, Type)> GetRequirements()
    {
        var returnValue = new List<(Type, Type)>();
        returnValue.Add((typeof(Stun), typeof(Health)));
        return returnValue;
    }

    public override Query ProcessQuery(Query action)
    {
        if (action.type == QueryType.Question)
        {
            if (action.parameters.ContainsKey(QueryParameter.CanAct)) {
                action.parameters[QueryParameter.CanAct] = 0;
            }
        }
        if (action.type == QueryType.Description)
        {
            if (action.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                action.Add(String.Format("Stunned: {0} turn(s)", timer));
            }
        }
        return action;
    }

    public override Query Tick()
    {
        timer -= 1;
        return new Query(QueryType.None);
    }
}