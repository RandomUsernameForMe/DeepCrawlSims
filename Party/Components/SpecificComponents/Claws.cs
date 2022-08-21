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

    public override Query ProcessQuery(Query query)
    {
        if (query.type == QueryType.AttackBuilder)
        {
            if (query.parameters.ContainsKey(QueryParameter.Special))
            {                
                query.Add(QueryParameter.Enemy, 1);
                query.Add(QueryParameter.Claws, new Clawed(strength,2));
                query.Add(QueryParameter.PhysDmg, strength);
            }
        }
        if (query.type == QueryType.Description)
        {
            if (query.parameters.ContainsKey(QueryParameter.Special))
            {
                query.Add("Cuts into damaged flesh. Does more damage to already clawed enemies.");
            }
            if (query.parameters.ContainsKey(QueryParameter.SpecialName))
            {
                query.Add("Claw attack");
            }
            if (query.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                query.Add(String.Format("Claws: {0} dmg, 2x to clawed enemies", strength));
            }
        }
        return query;
    }

    public override bool TryUpgrade(bool positive)
    {

        if (positive) strength += 5;
        else
        {
            if (strength <= 5)
            {
                return true;
            }
            strength -= 5;
        }
        return true;
    }
}

public class Clawed : TimedEffect
{
    int intensity;


    public Clawed(int str, int time)
    {
        intensity = str;
        timer = time;
    }
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
            if (query.effects.ContainsKey(QueryParameter.Claws))
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
}