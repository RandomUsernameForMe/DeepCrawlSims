using DeepCrawlSims.PartyNamespace.Components;
using DeepCrawlSims.QueryNamespace;
using System.Collections;
using System.Collections.Generic;

public abstract class TimedEffect : Component
{
    public int timer;
    public abstract Query Tick();

}
