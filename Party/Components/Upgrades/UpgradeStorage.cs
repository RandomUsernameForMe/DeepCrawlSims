using DeepCrawlSims.PartyNamespace;
using DeepCrawlSims.PartyNamespace.Components.Upgrades;
using System.Collections;
using System.Collections.Generic;

public class UpgradeStorage
{
    static List<UpgradeWithCondition> stored;

    public static List<UpgradeWithCondition> GetPositiveUpgrades()
    {

        if (stored == null)
        {
            var positiveUpgrades = new List<UpgradeWithCondition>();
            positiveUpgrades.Add(new UpgradeWithCondition(
                new ComponentCondition<Health>(), 
                new ComponentUpgrade<Health>(),
                2,
                "Raise Health", 
                "Raises maximum health by 10."));
            positiveUpgrades.Add(new UpgradeWithCondition(
                new ComponentCondition<Health>(), 
                new ComponentUpgrade<Armor>(),
                1,
                "Armor Up", 
                "You reduce physical damage by 3 more."));
            positiveUpgrades.Add(new UpgradeWithCondition(
                new ComponentCondition<PhysicalWeapon>(), 
                new ComponentUpgrade<PhysicalWeapon>(),
                2,
                "Sharpen Weapon", 
                "Your weapon will deal 5 more physical damage."));
            positiveUpgrades.Add(new UpgradeWithCondition(
                new ComponentCondition<PoisonBlast>(), 
                new ComponentUpgrade<PoisonBlast>(),
                2, 
                "Brew better poison", 
                "Increases strength or duration of your poison."));
            positiveUpgrades.Add(new UpgradeWithCondition(
                new ComponentCondition<HealingWave>(), 
                new ComponentUpgrade<HealingWave>(),
                2,
                "Improve Healing",
                "You heal for 10 more health"));
            
            positiveUpgrades.Add(new UpgradeWithCondition(
                new ComponentCondition<PowerStrike>(),
                new ComponentUpgrade<PowerStrike>(),
                2,
                "Upgrade PowerStrike",
                "Smash even more."));
            positiveUpgrades.Add(new UpgradeWithCondition(
                new ComponentCondition<Claws>(),
                new ComponentUpgrade<Claws>(),
                2,
                "Grow bigger claws",
                "Increases claws damage by 5."));
            positiveUpgrades.Add(new UpgradeWithCondition(
                new ComponentCondition<PhysicalWeapon>(),
                new ComponentUpgrade<FieryWeapons>(),
                2,
                "Enflame Weapons",
                "Set your weapons ablaze. Deal bonus fire damage that ignores armor."));
            positiveUpgrades.Add(new UpgradeWithCondition(
                new ComponentCondition<Health>(),
                new ComponentUpgrade<ElementalResistance>(),
                2,
                "Get elemental resistance",
                "Add 10% resistance against fire and posion."));
            positiveUpgrades.Add(new UpgradeWithCondition(
                new ComponentCondition<Speed>(),
                new ComponentUpgrade<Speed>(),
                2,
                "Get quicker",
                "Add +1 to your speed, making you play earlier."));

            stored = positiveUpgrades;
        }
        return stored;
    }
}
