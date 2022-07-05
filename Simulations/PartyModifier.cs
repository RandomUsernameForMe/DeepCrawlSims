using DeepCrawlSims.PartyNamespace;
using System;
using System.Collections;
using System.Collections.Generic;

public class PartyModifier
{

    List<UpgradeWithCondition> positiveUpgrades;
    List<UpgradeWithCondition> negativeUpgrades;

    public void Start()
    {
        positiveUpgrades = UpgradeStorage.GetPositiveUpgrades();
        negativeUpgrades = UpgradeStorage.GetNegativeUpgrades();
    }

    public Party ModifyPartyDifficulty(Party enemyParty, int upgradePoints)
    {
        bool makeEnemiesHarder = true;
        if (upgradePoints ==0) return enemyParty;
        if (upgradePoints <0 )
        {
            upgradePoints = -upgradePoints;
            makeEnemiesHarder = false;
        }
        while (upgradePoints > 0)
        {
            var rnd = new Random();
            int rndInt = rnd.Next(0, 4);
            ChangeDifficultyForCreature(enemyParty.Creatures[rndInt], makeEnemiesHarder,ref upgradePoints);
        }
        return enemyParty;
    }

    private void ChangeDifficultyForCreature(Creature creature, bool makeEnemiesHarder,ref int points)
    {
        List<UpgradeWithCondition> possibleUpgrades;
        if (makeEnemiesHarder)
        {
            possibleUpgrades = positiveUpgrades.FindAll(x => x.IsConditionPassed(creature));
        }
        else {
            possibleUpgrades = negativeUpgrades.FindAll(x => x.IsConditionPassed(creature));
        }
        if (possibleUpgrades.Count == 0) throw new Exception();
        for (int i = 0; i < possibleUpgrades.Count*2; i++)
        {
            var rnd = new Random();
            int rndInt = rnd.Next(0, possibleUpgrades.Count);

            var upgradeSuccesful = possibleUpgrades[rndInt].TryApplyUpgrade(creature, makeEnemiesHarder,false);
            if (upgradeSuccesful)
            {
                points -= possibleUpgrades[rndInt].Upgrade.cost;
                return;
            }
        }
        points--;
        return;
        
    }

   
}
