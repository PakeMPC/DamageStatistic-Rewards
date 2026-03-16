using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace DamageStatistic
{
    public static class DamageRewards
    {
        public static void CalculateAndGiveRewards(NPC boss, List<PlayerDamageRecord> records, double totalDamage)
        {
            if (!DamageConfigJson.Config.DamageReward || records.Count == 0) return;

            // Verificar si alguien hizo más del MaxDamagePercent
            bool abuse = records.Any(r => (r.Damage / totalDamage * 100) > DamageConfigJson.Config.MaxDamagePercent);

            if (abuse)
            {
                foreach (var p in TShock.Players.Where(p => p != null && p.Active))
                {
                    p.SendErrorMessage(DamageRewardsi18s.GetText(p, "NO_REWARD_OVER_DAMAGE"));
                }
                return;
            }

            long copperPer100 = ParseCoinString(DamageConfigJson.Config.MoneyPer100Damage);
            long totalRewardPool = (long)((totalDamage / 100.0) * copperPer100);

            // Ordenamos de mayor a menor y extraemos solo los mejores 3
            var top3 = records.OrderByDescending(r => r.Damage).Take(3).ToList();
            int[] percentages = { DamageConfigJson.Config.Top1Percent, DamageConfigJson.Config.Top2Percent, DamageConfigJson.Config.Top3Percent };

            for (int i = 0; i < top3.Count; i++)
            {
                long reward = (long)(totalRewardPool * (percentages[i] / 100.0));
                if (reward > 0)
                {
                    var tsplayer = TShock.Players.FirstOrDefault(p => p != null && p.Active && p.Name == top3[i].Name);
                    if (tsplayer != null)
                    {
                        GiveCoins(tsplayer, reward);
                        tsplayer.SaveServerCharacter();
                        string formatCoins = FormatCoins(reward);
                        tsplayer.SendSuccessMessage(DamageRewardsi18s.GetText(tsplayer, "REWARD_RECEIVED", formatCoins, i + 1));
                    }
                }
            }
        }

        private static long ParseCoinString(string input)
        {
            long total = 0;
            string currentNum = "";
            foreach (char c in input.ToLower())
            {
                if (char.IsDigit(c)) currentNum += c;
                else if (c == 'p' && currentNum.Length > 0) { total += long.Parse(currentNum) * 1000000; currentNum = ""; }
                else if (c == 'g' && currentNum.Length > 0) { total += long.Parse(currentNum) * 10000; currentNum = ""; }
                else if (c == 's' && currentNum.Length > 0) { total += long.Parse(currentNum) * 100; currentNum = ""; }
                else if (c == 'c' && currentNum.Length > 0) { total += long.Parse(currentNum); currentNum = ""; }
            }
            return total;
        }

        private static string FormatCoins(long copper)
        {
            if (copper <= 0) return "[i/s0:71]";
            var sb = new StringBuilder();

            long p = copper / 1000000; copper %= 1000000;
            long g = copper / 10000; copper %= 10000;
            long s = copper / 100;
            long c = copper % 100;

            if (p > 0) sb.Append($"[i/s{p}:74] ");
            if (g > 0) sb.Append($"[i/s{g}:73] ");
            if (s > 0) sb.Append($"[i/s{s}:72] ");
            if (c > 0) sb.Append($"[i/s{c}:71] ");

            return sb.ToString().Trim();
        }

        private static void GiveCoins(TSPlayer player, long copper)
        {
            long p = copper / 1000000; copper %= 1000000;
            long g = copper / 10000; copper %= 10000;
            long s = copper / 100;
            long c = copper % 100;

            if (p > 0) player.GiveItem(74, (int)p);
            if (g > 0) player.GiveItem(73, (int)g);
            if (s > 0) player.GiveItem(72, (int)s);
            if (c > 0) player.GiveItem(71, (int)c);
        }
    }
}
