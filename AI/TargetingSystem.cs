using DeepCrawlSims.BattleControl;
using DeepCrawlSims.Party;
using DeepCrawlSims.Query;
using System.Collections.Generic;

namespace DeepCrawlSims.AI
{
    class TargetingSystem
    {
        /// <summary>
        /// Returns available target positions for an ability. 
        /// </summary>
        /// <param name="pos">Grid coordinates plus a bool if ally or enemy side</param>
        /// <param name="casterIsEnemy">If the one asking is enemy or frieds</param>
        /// <returns></returns>
        public static List<int> PickViableTargets(List<QueryParameter> pos, bool casterIsEnemy)
        {
            List<int> list = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
            foreach (var item in pos)
            {
                switch (item)
                {
                    case QueryParameter.Close:
                        list.Remove(0);
                        list.Remove(1);
                        list.Remove(6);
                        list.Remove(7);
                        break;
                    case QueryParameter.Far:
                        list.Remove(0);
                        list.Remove(2);
                        list.Remove(5);
                        list.Remove(7);
                        break;
                    case QueryParameter.Ally:
                        var x = 4;
                        if (casterIsEnemy) x = 0;
                        list.Remove(x);
                        list.Remove(x + 1);
                        list.Remove(x + 2);
                        list.Remove(x + 3);
                        break;
                    case QueryParameter.Enemy:
                        var y = 0;
                        if (casterIsEnemy) y = 4;
                        list.Remove(y);
                        list.Remove(y + 1);
                        list.Remove(y + 2);
                        list.Remove(y + 3);
                        break;
                }
            }
            return list;
        }

        public static Creature GetCreatureByPosition(int pos, BattleManager manager)
        {
            var party = manager.allyParty;
            if (pos >= 4)
            {
                party = manager.enemyParty;
                pos = pos - 4;
            }
            return party.GetParty()[pos].GetComponentInChildren<Creature>();
        }
    }

}