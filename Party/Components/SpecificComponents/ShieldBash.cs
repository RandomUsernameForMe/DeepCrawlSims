using DeepCrawlSims.QueryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public class ShieldBash : UpgradableComponent
{
    public int bashDmg;
    public int stunDuration;
    public float chance;
    private int upgradelvl;
    public override List<(Type, Type)> GetRequirements()
    {
        var returnValue = new List<(Type, Type)>();
        returnValue.Add((typeof(ShieldBash), typeof(Health)));
        return returnValue;
    }

    public override Query ProcessQuery(Query query)
    {
        if (query.type == QueryType.AttackBuild)
        {
            if (query.parameters.ContainsKey(QueryParameter.Special))
            {
                query.Add(QueryParameter.Close, 1);
                query.Add(QueryParameter.Enemy, 1);
                query.Add(QueryParameter.PhysDmg, bashDmg);
                query.Add(QueryParameter.Stun,new Stun(stunDuration));
            }
        }
        if (query.type == QueryType.Description)
        {
            if (query.parameters.ContainsKey(QueryParameter.Special))
            {
                query.Add("Bashes enemy with a shield. Stuns.");
            }
            if (query.parameters.ContainsKey(QueryParameter.SpecialName))
            {
                query.Add("Shield Bash");
            }
            if (query.parameters.ContainsKey(QueryParameter.Tooltip))
            {
                query.Add(String.Format("Bash: {0} dmg, {1} turn stun", bashDmg, stunDuration));
            }
        }
        return query;
    }

    public override bool TryUpgrade(bool positive)
    {
        int newlvl = upgradelvl;
        if (positive) newlvl++;
        else newlvl--;

        switch (newlvl) {
            case 0:
                //Destroy(this);
                return true;
            case 1:
                stunDuration = 1;
                bashDmg = 10;
                break;
            case 2:
                stunDuration = 1;
                bashDmg = 20;
                break;
            case 3:
                stunDuration = 2;
                bashDmg = 25;
                break;
            case 4:
                return false;
        }
        upgradelvl = newlvl;
        return true;
    }
}
