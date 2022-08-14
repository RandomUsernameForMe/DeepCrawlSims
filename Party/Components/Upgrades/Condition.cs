using DeepCrawlSims.PartyNamespace;
using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Condition
{
    public string name;
    public abstract bool isPassed();
}

public abstract class CreatureCondition : Condition
{
    public Creature creature;
} 


/// <summary>
/// Checks whether an object of specific name end cantains a specified StatusEffect
/// </summary>
public class ComponentCondition<T> : CreatureCondition
{
    private bool boolHelper = true;
    public ComponentCondition(Creature creature)
    {
        this.creature = creature;
    }
    public ComponentCondition(string name = "",bool boolean = true)
    {
        this.name = name;
        this.boolHelper = boolean;
    }

    override public bool isPassed()
    {
        foreach (var item in creature.components)
        {
            if (item is T) return true;
        }
        return false;
    }
}

/// <summary>
/// Checks whether an object of specific name contains a specific value of specific parameter. For example if health equals exactly 0.
/// </summary>
public class HasValue : CreatureCondition
{
    public QueryParameter parameter;
    public double value;

    public HasValue(QueryParameter parameter, double value, string name)
    {
        this.parameter = parameter;
        this.value = value;
        this.name = name;
    }

    public override bool isPassed()
    {
        var query = new Query(QueryType.Question);
        query.Add(parameter, 0);
        query = creature.ProcessQuery(query);
        return (query.parameters[parameter] == value);
    }
}