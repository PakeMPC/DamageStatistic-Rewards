using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace DamageStatistic
{
    public class DamageConfig
    {
        public bool ShowDamage { get; set; } = true;
        public bool DamageReward { get; set; } = true;
        public string MoneyPer100Damage { get; set; } = "20s";
        public int Top3Percent { get; set; } = 10;
        public int Top2Percent { get; set; } = 30;
        public int Top1Percent { get; set; } = 60;
        public int MaxDamagePercent { get; set; } = 80;
        public string DefaultLanguage { get; set; } = "en";
        public Dictionary<string, string> PlayerLanguages { get; set; } = new Dictionary<string, string>();
    }

    public static class DamageConfigJson
    {
        public static DamageConfig Config = new DamageConfig();
        private static string filePath = Path.Combine(TShock.SavePath, "BossDamage&Rewards.json");

        public static void Load()
        {
            try
            {
                if (!File.Exists(filePath)) Save();
                else
                {
                    string json = File.ReadAllText(filePath);
                    Config = JsonConvert.DeserializeObject<DamageConfig>(json) ?? new DamageConfig();
                }

                DamageRewardsi18s.PlayerLanguages = Config.PlayerLanguages ?? new Dictionary<string, string>();
            }
            catch (Exception ex) { TShock.Log.Error("[DamageStatistic] Load Error: " + ex.Message); }
        }

        public static void Save()
        {
            try
            {
                Config.PlayerLanguages = DamageRewardsi18s.PlayerLanguages;
                string json = JsonConvert.SerializeObject(Config, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex) { TShock.Log.Error("[DamageStatistic] Save Error: " + ex.Message); }
        }
    }
}
