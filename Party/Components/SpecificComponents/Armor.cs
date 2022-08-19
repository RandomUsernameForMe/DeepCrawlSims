using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public class Armor : UpgradableComponent
{
    public int armor;

    public Armor(int v)
    {
        armor = v;
    }

    public Armor()
    {
        armor = 3;
    }

    override public Query ProcessQuery(Query query)
    {
        if (query.type == QueryType.Attack || query.type == QueryType.Question)
        {
            if (query.parameters.ContainsKey(QueryParameter.PhysDmg))
            {                
                query.parameters[QueryParameter.PhysDmg] -= armor;
            } 
        }
        if (query.type == QueryType.Description)
        {            
            if (query.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                query.Add(String.Format("Armor: {0}", armor));
            }
        }
        return query;
    }

    public override List<(Type, Type)> GetRequirements()
    {
        var returnValue = new List<(Type, Type)>();
        returnValue.Add((typeof(Armor), typeof(Health)));
        return returnValue;
    }

    public override bool TryUpgrade(bool positive)
    {

        if (positive) armor += 3;
        else
        {
            if (armor <= 3)
            {
                //Destroy(this);
                return true;
            }
            armor -= 3;
        }
        
        return true;
    }
}
