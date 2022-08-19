using DeepCrawlSims.PartyNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

public class PartyModifier
{

    List<UpgradeWithCondition> upgrades;

    public void Start()
    {
        upgrades = UpgradeStorage.GetPositiveUpgrades();
    }

    public Party ModifyPartyDifficulty(Party enemyParty, int upgradePoints)
    {
        bool makeEnemiesHarder = true;
        if (upgradePoints == 0) return enemyParty;
        if (upgradePoints < 0)
        {
            upgradePoints = -upgradePoints;
            makeEnemiesHarder = false;
        }
        while (upgradePoints > 0)
        {
            var rnd = new Random();
            int rndInt = rnd.Next(0, 4);
            ChangeDifficultyForCreature(enemyParty.Creatures[rndInt], makeEnemiesHarder, ref upgradePoints);
        }
        return enemyParty;
    }

    private void ChangeDifficultyForCreature(Creature creature, bool makeEnemiesHarder, ref int points)
    {
        List<UpgradeWithCondition> possibleUpgrades;
        if (makeEnemiesHarder)
        {
            possibleUpgrades = upgrades.FindAll(x => x.IsConditionPassed(creature));
        }
        else
        {
            possibleUpgrades = null; //TODO;
        }
        if (possibleUpgrades.Count == 0) throw new Exception();
        for (int i = 0; i < possibleUpgrades.Count * 2; i++)
        {
            var rnd = new Random();
            int rndInt = rnd.Next(0, possibleUpgrades.Count);

            possibleUpgrades[rndInt].ApplyUpgrade(creature, makeEnemiesHarder, false);
            points -= possibleUpgrades[rndInt].Upgrade.cost;
            return;
        }
        points--;
        return;

    }


}
