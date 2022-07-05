using DeepCrawlSims.PartyNamespace;
using System.Collections;
using System.Collections.Generic;

public class UpgradeWithCondition
{
    public Upgrade Upgrade { get; }
    public CreatureCondition Condition { get; }

    public UpgradeWithCondition(CreatureCondition cond, Upgrade upg)
    {
        Condition = cond;
        Upgrade = upg;
    }

    public UpgradeWithCondition(CreatureCondition cond, Upgrade upg,int cost, string buttonText, string descriptionText)
    {
        Condition = cond;
        Upgrade = upg;
        Upgrade.cost = cost;
        Upgrade.buttonText = buttonText;
        Upgrade.descriptionText = descriptionText;
    }

    public UpgradeWithCondition(CreatureCondition cond, Upgrade upg, int cost)
    {
        Condition = cond;
        Upgrade = upg;
        Upgrade.cost = cost;
    }

    public bool IsConditionPassed(Creature creature)
    {
        Condition.creature = creature;
        return Condition.isPassed();
    }

    public bool TryApplyUpgrade(Creature creature, bool positive, bool unlimitedSpace)
    {
        return Upgrade.TryUpgrade(creature,positive,unlimitedSpace);
    }
}
