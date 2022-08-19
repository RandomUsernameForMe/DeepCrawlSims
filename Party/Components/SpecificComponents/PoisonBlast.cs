using System.Collections;
using System.Collections.Generic;
using System;
using DeepCrawlSims.QueryNamespace;

[Serializable()]
public class PoisonBlast: UpgradableComponent
{
    public int potency = 10;
    public int duration = 2;
    private int upgradeLevel = 1;

    public override Query ProcessQuery(Query action)
    {
        if (action.type == QueryType.AttackBuild)
        {
            if (action.parameters.ContainsKey(QueryParameter.Special))
            {
                action.Add(QueryParameter.Poison,new Poison(potency, duration));
            }
        }
        if (action.type == QueryType.Description)
        {
            if (action.parameters.ContainsKey(QueryParameter.SpecialName))
            {
                action.Add("Poison Blast");
            }
            if (action.parameters.ContainsKey(QueryParameter.Special))
            {
                action.Add("Blasts an enemy with a powerful toxin.");
            }
            if (action.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                action.Add(String.Format("Poison: {0} dmg, {1} turn(s)", potency, duration));
            }
        }
        return action;
    }

    public override List<(Type,Type)> GetRequirements()
    {
        var returnValue = new List<(Type, Type)>();
        returnValue.Add((typeof(PoisonBlast), typeof(Health)));
        return returnValue;
    }


        public override bool TryUpgrade(bool positive)
    {
        int newlvl = upgradeLevel;
        if (positive) newlvl++;
        else newlvl--;

        switch (newlvl)
        {
            case 0:
                return true;
            case 1:
                duration = 2;
                potency = 10;
                break;
            case 2:
                duration = 2;
                potency = 20;
                break;
            case 3:
                duration = 100;
                potency = 20;
                break;
            case 4:
                return false;
        }
        upgradeLevel = newlvl;
        return true;
    }

}

public class Poison : TimedEffect
{
    public double potency;

    public Poison(double potency, int poisonTimer)
    {
        this.potency = potency;
        this.timer = poisonTimer;
    }


    public override List<(Type, Type)> GetRequirements()
    {
        return null;
    }

    public override Query ProcessQuery(Query query)
    {
        if (query.type == QueryType.Description)
        {
            if (query.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                query.Add(String.Format("Poisoned: {0} dmg, {1} turns", potency, timer));
            }
        }
        if (query.type == QueryType.Attack)
        {
            if (query.effects.ContainsKey(QueryParameter.Poison))
            {
                timer = Math.Max(2,timer);
                potency = Math.Max(potency, (query.effects[QueryParameter.Poison] as Poison).potency);
                query.effects.Remove(QueryParameter.Poison);
            }
            if (query.parameters.ContainsKey(QueryParameter.PoisonAmp))
            {
                potency += 5;
                timer += 1;
            }
        }
        return query;
    }

    public override Query Tick()
    {
        Query action = new Query(QueryType.Attack);
        action.Add(QueryParameter.TrueDmg, potency);
        timer -= 1;
        return action;
    }

    public void Set(double potency, int duration)
    {
        this.potency = potency;
        this.timer = duration;
        active = true;
    }
}

