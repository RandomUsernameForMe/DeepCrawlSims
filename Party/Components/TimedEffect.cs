using DeepCrawlSims.PartyNamespace.Components;
using DeepCrawlSims.QueryNamespace;
using System.Collections;
using System.Collections.Generic;

public abstract class TimedEffect : Component
{
    public int timer;
    public bool active;
    public abstract Query Tick();

}
