using DeepCrawlSims.PartyNamespace;
using System;

namespace DeepCrawlSims
{
    internal class TeamBuilder
    {

        internal static Party CreateEnemyTeam()
        {
            var party = new Party();
            party.Creatures.Add(GeneratePoisonMage("Mage1"));
            party.Creatures.Add(GenerateFighter("Fighter1"));
            party.Creatures.Add(GenerateClawer("Clawer1"));
            party.Creatures.Add(GenerateClawer("Clawer2"));
            foreach (var item in party.Creatures) { item.isEnemy = false; }

            return party;
        }

        internal static Party CreateAllyTeam()
        {
            var party = new Party();
            party.Creatures.Add(GeneratePoisonMage("Mage1"));
            party.Creatures.Add(GenerateHealer("Healer1"));
            party.Creatures.Add(GenerateFighter("Fighter1"));
            party.Creatures.Add(GenerateFighter("Fighter2"));
            foreach (var item in party.Creatures) { item.isEnemy = false; }

            return party;
        }

        private static Creature GeneratePoisonMage(string name)
        {
            var creature = new Creature(name,false);
            creature.components.Add(new PhysicalWeapon());
            creature.components.Add(new LongWeapon());
            creature.components.Add(new PoisonBlast());
            creature.components.Add(new Health(50));
            creature.components.Add(new Armor(3));
            creature.components.Add(new Speed(3));
            return creature;
        }

        private static Creature GenerateFighter(string name)
        {
            var creature = new Creature(name, false);
            creature.components.Add(new PhysicalWeapon());
            creature.components.Add(new LongWeapon());
            creature.components.Add(new PowerStrike());
            creature.components.Add(new Health(50));
            creature.components.Add(new Armor(5));
            creature.components.Add(new Speed(2));
            return creature;
        }

        private static Creature GenerateClawer(string name)
        {
            var creature = new Creature(name, false);
            creature.components.Add(new PhysicalWeapon());
            creature.components.Add(new LongWeapon());
            creature.components.Add(new Claws());
            creature.components.Add(new Health(50));
            creature.components.Add(new Armor(5));
            creature.components.Add(new Speed(2));
            return creature;
        }

        private static Creature GenerateHealer(string name)
        {
            var creature = new Creature(name, false);
            creature.components.Add(new PhysicalWeapon());
            creature.components.Add(new LongWeapon());
            creature.components.Add(new HealingWave());
            creature.components.Add(new Health(50));
            creature.components.Add(new Armor(5));
            creature.components.Add(new Speed(3));
            return creature;
        }
    }
}