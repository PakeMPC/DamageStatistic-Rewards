using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace DamageStatistic
{
    public class BossRecord
    {
        public int NpcId;
        public DateTime StartTime;
        public Dictionary<string, int> PlayerDamage = new Dictionary<string, int>();
    }

    public class PlayerDamageRecord
    {
        public string Name;
        public int Damage;
    }

    [ApiVersion(2, 1)]
    public class DamageStatistic : TerrariaPlugin
    {
        public override string Name => "DamageStatistic&Rewards";
        public override Version Version => new Version(2, 0);
        public override string Author => "Megghy & PakeMPC";
        public override string Description => "Show the damage caused by each player after each boss battle and gives rewards";

        private Dictionary<int, BossRecord> ActiveBosses = new Dictionary<int, BossRecord>();

        public DamageStatistic(Main game) : base(game) { }

        public override void Initialize()
        {
            DamageConfigJson.Load();
            ServerApi.Hooks.NpcStrike.Register(this, OnStrike);
            ServerApi.Hooks.NpcKilled.Register(this, OnNpcKill);
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            Commands.ChatCommands.Add(new Command("damagestatistic.lang", LangCmd, "damagelang"));
            TShockAPI.Hooks.GeneralHooks.ReloadEvent += OnReload;
        }

        private void OnReload(TShockAPI.Hooks.ReloadEventArgs args)
        {
            DamageConfigJson.Load();

            args.Player.SendSuccessMessage("[DamageStatistic&Rewards] CONFIG FILE CHARGED");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NpcStrike.Deregister(this, OnStrike);
                ServerApi.Hooks.NpcKilled.Deregister(this, OnNpcKill);
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
                ActiveBosses.Clear();

                Commands.ChatCommands.RemoveAll(c => c.HasAlias("damagelang"));
                TShockAPI.Hooks.GeneralHooks.ReloadEvent -= OnReload;

            }
            base.Dispose(disposing);
        }

        private void LangCmd(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
                DamageRewardsi18s.ChangeLanguage(args.Player, args.Parameters[0].ToLower());
            else
                args.Player.SendErrorMessage("Uso: /damagelang <es|en|pt>");
        }

        private void OnUpdate(EventArgs args)
        {
            if (Main.GameUpdateCount % 300 != 0) return;

            var keys = ActiveBosses.Keys.ToList();
            foreach (var k in keys)
            {
                if (!Main.npc[k].active || (!Main.npc[k].boss && !IsBossPart(Main.npc[k])))
                {
                    ActiveBosses.Remove(k);
                }
            }
        }

        private void OnStrike(NpcStrikeEventArgs args)
        {
            if (args.Npc == null || !args.Npc.active || (!args.Npc.boss && !IsBossPart(args.Npc))) return;

            if (args.Player == null) return;

            int bossKey = args.Npc.realLife != -1 ? args.Npc.realLife : args.Npc.whoAmI;

            if (!ActiveBosses.TryGetValue(bossKey, out var tracker))
            {
                tracker = new BossRecord { NpcId = bossKey, StartTime = DateTime.UtcNow };
                ActiveBosses[bossKey] = tracker;
            }

            string pName = args.Player.name;
            if (!tracker.PlayerDamage.ContainsKey(pName)) tracker.PlayerDamage[pName] = 0;
            tracker.PlayerDamage[pName] += args.Damage;
        }

        private void OnNpcKill(NpcKilledEventArgs args)
        {
            if (args.npc == null) return;

            int bossKey = args.npc.realLife != -1 ? args.npc.realLife : args.npc.whoAmI;

            if (args.npc.realLife != -1 || IsBossPart(args.npc))
            {
                bool bossStillAlive = Main.npc.Any(n =>
                    n.active &&
                    n.whoAmI != args.npc.whoAmI &&
                    (n.realLife == bossKey || (n.type == args.npc.type && IsBossPart(n)))
                );

                if (bossStillAlive) return;
            }

            if (ActiveBosses.TryGetValue(bossKey, out var tracker))
            {
                ProcessBossKill(args.npc, tracker);
                ActiveBosses.Remove(bossKey);
            }
        }

        private void ProcessBossKill(NPC npc, BossRecord tracker)
        {

            double totalDamage = tracker.PlayerDamage.Values.Sum();
            if (totalDamage <= 0) return;

            double totalSeconds = (DateTime.UtcNow - tracker.StartTime).TotalSeconds;
            if (totalSeconds < 1) totalSeconds = 1;

            var onlineRecords = tracker.PlayerDamage
                .Where(kvp => kvp.Value > 0 && TShock.Players.Any(p => p != null && p.Active && p.Name == kvp.Key))
                .Select(kvp => new PlayerDamageRecord { Name = kvp.Key, Damage = kvp.Value })
                .OrderBy(x => x.Damage)
                .ToList();

            if (onlineRecords.Count == 0) return;

            if (DamageConfigJson.Config.ShowDamage)
            {
                foreach (var player in TShock.Players.Where(p => p != null && p.Active))
                {
                    for (int i = 0; i < onlineRecords.Count; i++)
                    {
                        int rank = onlineRecords.Count - i;
                        var rec = onlineRecords[i];
                        double pct = (rec.Damage / totalDamage) * 100;
                        int dps = (int)(rec.Damage / totalSeconds);

                        string nameFmt = rec.Name;
                        if (rank == 1) nameFmt = Rainbow(rec.Name);
                        else if (rank == 2) nameFmt = $"[c/808080:{rec.Name}]";
                        else if (rank == 3) nameFmt = $"[c/FFA500:{rec.Name}]";

                        if (rank <= 3)
                        {
                            string key = $"RANK_{rank}";
                            player.SendMessage(DamageRewardsi18s.GetText(player, key, nameFmt, rec.Damage, pct.ToString("0.00"), dps), Color.White);
                        }
                        else
                        {
                            player.SendMessage(DamageRewardsi18s.GetText(player, "RANK_FORMAT", rank, nameFmt, rec.Damage, pct.ToString("0.00"), dps), Color.White);
                        }
                    }
                    player.SendMessage(DamageRewardsi18s.GetText(player, "TOP_DAMAGE_HEADER", npc.FullName.ToUpper()), Color.LimeGreen);
                }
            }

            // Procesar Recompensas
            if (DamageConfigJson.Config.DamageReward)
            {
                DamageRewards.CalculateAndGiveRewards(npc, onlineRecords, totalDamage);
            }
        }

        private bool IsBossPart(NPC npc)
        {
            // Partes del Eater of Worlds y otros que TShock a veces no marca como jefe principal
            return npc.type == 13 || npc.type == 14 || npc.type == 15 || npc.type == 398;
        }

        // Utilidad para imprimir el color arcoíris
        private string Rainbow(string text)
        {
            string[] colors = { "FF0000", "FF7F00", "FFFF00", "00FF00", "0000FF", "4B0082", "9400D3" };
            var sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ') sb.Append(" ");
                else sb.Append($"[c/{colors[i % colors.Length]}:{text[i]}]");
            }
            return sb.ToString();
        }
    }
}
