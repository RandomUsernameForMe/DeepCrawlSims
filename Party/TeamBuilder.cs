using DeepCrawlSims.PartyNamespace;
using System;

namespace DeepCrawlSims
{
    internal class TeamBuilder
    {

        internal static Party CreateEnemyTeam()
        {
            return CreateTeam(true);
        }

        internal static Party CreateAllyTeam()
        {
            return CreateTeam(false);
        }

        internal static Party CreateTeam(bool isEnemy)
        {
            var party = new Party();
            party.Creatures.Add(GeneratePoisonMage("Mage1"));
            party.Creatures.Add(GeneratePoisonMage("Mage2"));
            party.Creatures.Add(GenerateFighter("Fighter1"));
            party.Creatures.Add(GenerateFighter("Fighter2"));
            foreach (var item in party.Creatures) { item.isEnemy = isEnemy;}

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
            return creature;
        }
    }
}