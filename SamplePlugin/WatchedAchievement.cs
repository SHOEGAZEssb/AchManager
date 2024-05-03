using ECommons.DalamudServices;
using ECommons.EzHookManager;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchManager
{
    internal unsafe class WatchedAchievement
    {
        public delegate void ReceiveAchievementProgressDelegate(Achievement* achievement, uint id, uint current, uint max);
        public EzHook<ReceiveAchievementProgressDelegate> ReceiveAchievementProgressHook;

        public AchievementUpdateTrigger? Trigger
        {
            get => trigger;
            set
            {
                if (trigger != value) 
                { 
                    if (trigger != null)
                        trigger.OnTrigger -= Trigger_OnTrigger;

                    if (value != null)
                        value.OnTrigger += Trigger_OnTrigger;

                    trigger = value;
                }
            }
        }
        private AchievementUpdateTrigger? trigger;

        public uint Progress { get; private set; }

        private readonly uint watchedID;
        private readonly Lumina.Excel.GeneratedSheets.Achievement achievementInfo;

        public WatchedAchievement(uint id)
        {
            watchedID = id;
            achievementInfo = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Achievement>().FirstOrDefault(a => a.RowId == watchedID);          
            ReceiveAchievementProgressHook = new EzHook<ReceiveAchievementProgressDelegate>(Achievement.Addresses.ReceiveAchievementProgress.String, ReceiveAchievementProgressDetour);
            ReceiveAchievementProgressHook.Enable();
        }

        private void Trigger_OnTrigger(object? sender, EventArgs e)
        {
            Achievement.Instance()->RequestAchievementProgress(watchedID);
        }

        private void ReceiveAchievementProgressDetour(Achievement* achievement, uint id, uint current, uint max)
        {
            if (id == watchedID)
            {
                if (Progress != current)
                {
                    Svc.Log.Debug($"Achievement progress: {current}/{max}");
                    Svc.Chat.Print($"Achievement '{achievementInfo.Name}' Progress: {current}/{max}");
                }
                
                Progress = current;
            }
            ReceiveAchievementProgressHook.Original(achievement, id, current, max);
        }
    }
}
