using DeepCrawlSims.PartyNamespace.Components;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public abstract class UpgradableComponent : Component
{
    public abstract bool TryUpgrade(bool positive);
}
