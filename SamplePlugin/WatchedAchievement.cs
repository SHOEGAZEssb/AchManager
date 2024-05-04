using AchManager.AchievementTrigger;
using ECommons.DalamudServices;
using ECommons.EzHookManager;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Linq;

namespace AchManager
{
  internal unsafe class WatchedAchievement : IDisposable
  {
    public delegate void ReceiveAchievementProgressDelegate(Achievement* achievement, uint id, uint current, uint max);
    public EzHook<ReceiveAchievementProgressDelegate> ReceiveAchievementProgressHook;

    public AchievementUpdateTriggerBase? Trigger
    {
      get => trigger;
      set
      {
        if (trigger != value)
        {
          if (trigger != null)
          {
            trigger.OnTrigger -= Trigger_OnTrigger;
            trigger.Dispose();
          }

          if (value != null)
            value.OnTrigger += Trigger_OnTrigger;

          trigger = value;
        }
      }
    }
    private AchievementUpdateTriggerBase? trigger;

    public uint Progress { get; private set; }

    public uint WatchedID { get; private set; }
    private readonly Lumina.Excel.GeneratedSheets.Achievement? _achievementInfo;

    public WatchedAchievement(uint id, AchievementUpdateTriggerBase? trigger)
    {
      WatchedID = id;
      Trigger = trigger;
      _achievementInfo = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Achievement>()?.FirstOrDefault(a => a.RowId == WatchedID);
      ReceiveAchievementProgressHook = new EzHook<ReceiveAchievementProgressDelegate>(Achievement.Addresses.ReceiveAchievementProgress.String, ReceiveAchievementProgressDetour);
      ReceiveAchievementProgressHook.Enable();
    }

    private void Trigger_OnTrigger(object? sender, EventArgs e)
    {
      Achievement.Instance()->RequestAchievementProgress(WatchedID);
    }

    private void ReceiveAchievementProgressDetour(Achievement* achievement, uint id, uint current, uint max)
    {
      if (id == WatchedID)
      {
        if (Progress != current)
        {
          Svc.Log.Debug($"Achievement progress: {current}/{max}");
          Svc.Chat.Print($"Achievement '{_achievementInfo?.Name}' Progress: {current}/{max}");
        }

        Progress = current;
      }
      ReceiveAchievementProgressHook.Original(achievement, id, current, max);
    }

    public void Dispose()
    {
      ReceiveAchievementProgressHook.Disable();
      Trigger?.Dispose();
      Trigger = null;
    }
  }
}
