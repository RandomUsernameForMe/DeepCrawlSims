using DeepCrawlSims.PartyNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Upgrade : IComparable
{
    public string buttonText;
    public string descriptionText;
    public int cost;

    public int CompareTo(object obj)
    {
        return buttonText.ToString().CompareTo(obj.ToString());
    }

    abstract public void UpgradeOrAdd(Creature creature, bool positive,bool unlimitedSpace);
}