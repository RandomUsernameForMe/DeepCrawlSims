using DeepCrawlSims.PartyNamespace;
using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public class Claws : UpgradableComponent
{
    public int strength = 10;
    public override List<(Type, Type)> GetRequirements()
    {
        var returnValue = new List<(Type, Type)>();
        returnValue.Add((typeof(Claws), typeof(Health)));
        return returnValue;
    }

    public override Query ProcessQuery(Query action)
    {
        if (action.type == QueryType.AttackBuild)
        {
            if (action.parameters.ContainsKey(QueryParameter.Special))
            {                
                action.Add(QueryParameter.Enemy, 1);
                action.Add(QueryParameter.Claws, new Clawed());
            }
        }
        if (action.type == QueryType.Description)
        {
            if (action.parameters.ContainsKey(QueryParameter.Special))
            {
                action.Add("Cuts into damaged flesh. Does more damage to already clawed enemies.");
            }
            if (action.parameters.ContainsKey(QueryParameter.SpecialName))
            {
                action.Add("Claw attack");
            }
            if (action.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                action.Add(String.Format("Claws: {0} dmg, 2x to clawed enemies", strength));
            }
        }
        return action;
    }

    public override bool TryUpgrade(bool positive)
    {
        if (strength <= 5)
        {
            //Destroy(this);
            return true;
        }
        if (positive) strength += 5;
        else strength -= 5;
        return true;
    }
}

public class Clawed : TimedEffect
{
    int intensity;
    public override List<(Type, Type)> GetRequirements()
    {
        var returnValue = new List<(Type, Type)>();
        returnValue.Add((typeof(Claws), typeof(Armor)));
        return returnValue;
    }

    public override Query ProcessQuery(Query query)
    {
        if (query.type == QueryType.Attack)
        {
            if (query.parameters.ContainsKey(QueryParameter.Claws))
            {
                query.parameters[QueryParameter.PhysDmg] += intensity;
                timer++;
                query.effects.Remove(QueryParameter.Claws);
            }
        }
        if (query.type == QueryType.Description)
        {
            if (query.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                query.Add(String.Format("Clawed: recieves extra {0} dmg, {1} turns", intensity, timer));
            }
        }
        return query;
    }

    public override Query Tick()
    {
        timer = timer - 1;
        return new Query(QueryType.None);
    }

    internal void Set(int strength)
    {
        intensity = strength;
        timer = 3;
    }
}