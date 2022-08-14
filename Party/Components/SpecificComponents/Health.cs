using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public class Health : UpgradableComponent
{
    public double health = 50;
    public int maxHealth = 50;

    public Health(int health) {
        this.health = health;
        maxHealth = health;
    }

    override public Query ProcessQuery(Query query)
    {
        double healthChange =0 ;
        if (query.type == QueryType.Attack || query.type == QueryType.Question)
        {
            if (query.parameters.ContainsKey(QueryParameter.PhysDmg))
            {
                healthChange = -query.parameters[QueryParameter.PhysDmg];
            }
            if (query.parameters.ContainsKey(QueryParameter.PercentDmg))
            {
                healthChange = -query.parameters[QueryParameter.PercentDmg] * health;
            }
            if (query.parameters.ContainsKey(QueryParameter.TrueDmg))
            {
                healthChange = -query.parameters[QueryParameter.TrueDmg];
            }
            if (query.parameters.ContainsKey(QueryParameter.Healing))
            {
                healthChange = Math.Min(query.parameters[QueryParameter.Healing], maxHealth-health);
            }
        }
        
        if (query.type == QueryType.Attack)
        {
            health += healthChange;            
        }
        
        if (query.type == QueryType.Question)
        {            
            query.Add(QueryParameter.CalcultedDmg, -healthChange);
        }

        if (query.type == QueryType.Question)
        {
            if (query.parameters.ContainsKey(QueryParameter.CanAct) && health > 0)
            {
                query.parameters[QueryParameter.CanAct] = 1;
            }
            if (query.parameters.ContainsKey(QueryParameter.Dead) && health <= 0)
            {
                query.parameters[QueryParameter.Dead] = 1;
            }
        }
        if (query.type == QueryType.Description)
        {
            if (query.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                query.Add(String.Format("Health: {0}", maxHealth));
            }
        }
        return query;
    }

    public void Heal()
    {
        health = maxHealth;
    }

    public override List<(Type, Type)> GetRequirements()
    {
        return null;
    }

    public override bool TryUpgrade(bool positive)
    {
        if (maxHealth <= 20) return false;
        if (positive) maxHealth += 10;
        else maxHealth -= 10;
        return true;
    }
}
