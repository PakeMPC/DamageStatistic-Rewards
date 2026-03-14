using System.Collections.Generic;
using TShockAPI;

namespace DamageStatistic
{
    public static class DamageRewardsi18s
    {
        public static Dictionary<string, string> PlayerLanguages = new Dictionary<string, string>();

        private static readonly Dictionary<string, Dictionary<string, string>> Languages = new Dictionary<string, Dictionary<string, string>>
        {
            ["es"] = new Dictionary<string, string> {
                { "RANK_FORMAT", "{0}. {1} {2} ({3}%) (DPS:{4})" },
                { "RANK_3", "3[i:4599] {0} {1} ({2}%) (DPS:{3})" },
                { "RANK_2", "2[i:4600] {0} {1} ({2}%) (DPS:{3})" },
                { "RANK_1", "1[i:4601] {0} {1} ({2}%) (DPS:{3})" },
                { "TOP_DAMAGE_HEADER", "--[i:4601]TOP DAÑO A {0}[i:4601]--" },
                { "NO_REWARD_OVER_DAMAGE", "Sin recompensas: Límite de farmeo de daño excedido." },
                { "REWARD_RECEIVED", "Recompensa de {0} por quedar en top {1}." },
                { "LANG_CHANGED", "Idioma cambiado a {0}." }
            },
            ["en"] = new Dictionary<string, string> {
                { "RANK_FORMAT", "{0}. {1} {2} ({3}%) (DPS:{4})" },
                { "RANK_3", "3[i:4599] {0} {1} ({2}%) (DPS:{3})" },
                { "RANK_2", "2[i:4600] {0} {1} ({2}%) (DPS:{3})" },
                { "RANK_1", "1[i:4601] {0} {1} ({2}%) (DPS:{3})" },
                { "TOP_DAMAGE_HEADER", "--[i:4601]TOP DAMAGE TO {0}[i:4601]--" },
                { "NO_REWARD_OVER_DAMAGE", "No rewards: Damage farming limit exceeded." },
                { "REWARD_RECEIVED", "Reward of {0} for reaching top {1}." },
                { "LANG_CHANGED", "Language changed to {0}." }
            },
            ["pt"] = new Dictionary<string, string> {
                { "RANK_FORMAT", "{0}. {1} {2} ({3}%) (DPS:{4})" },
                { "RANK_3", "3[i:4599] {0} {1} ({2}%) (DPS:{3})" },
                { "RANK_2", "2[i:4600] {0} {1} ({2}%) (DPS:{3})" },
                { "RANK_1", "1[i:4601] {0} {1} ({2}%) (DPS:{3})" },
                { "TOP_DAMAGE_HEADER", "--[i:4601]DANO MÁXIMO PARA {0}[i:4601]--" },
                { "NO_REWARD_OVER_DAMAGE", "Sem recompensas: Limite de dano excedido." },
                { "REWARD_RECEIVED", "Recompensa de {0} por ficar no top {1}." },
                { "LANG_CHANGED", "Idioma alterado para {0}." }
            }
        };


        public static string GetText(TSPlayer player, string key, params object[] args)
        {
            string lang = DamageConfigJson.Config.DefaultLanguage;

            if (player?.Account != null && PlayerLanguages.TryGetValue(player.Account.Name, out string pLang))
            {
                lang = pLang;
            }

            if (!Languages.ContainsKey(lang))
            {
                lang = "en";
            }

            if (Languages.ContainsKey(lang) && Languages[lang].ContainsKey(key))
            {
                return string.Format(Languages[lang][key], args);
            }

            return key; 
        }

        public static void ChangeLanguage(TSPlayer player, string lang)
        {
            if (Languages.ContainsKey(lang) && player?.Account != null)
            {
                PlayerLanguages[player.Account.Name] = lang;
                player.SendSuccessMessage(GetText(player, "LANG_CHANGED", lang));
                DamageConfigJson.Save();
            }
        }
    }
}
