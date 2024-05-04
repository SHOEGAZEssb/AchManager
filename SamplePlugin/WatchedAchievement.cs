using AchManager.AchievementTrigger;
using ECommons.DalamudServices;
using ECommons.EzHookManager;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Linq;
using static Dalamud.Interface.Utility.Raii.ImRaii;

namespace AchManager
{
  internal unsafe class WatchedAchievement : IDisposable
  {
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
      AchievementHookManager.OnAchievementProgress += AchievementHookManager_OnAchievementProgress;
      WatchedID = id;
      Trigger = trigger;
      _achievementInfo = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Achievement>()?.FirstOrDefault(a => a.RowId == WatchedID);

    }

    private void AchievementHookManager_OnAchievementProgress(object? sender, AchievementProgressEventArgs e)
    {
      if (e.ID == WatchedID)
      {
        if (Progress != e.Progress)
        {
          Svc.Chat.Print($"Achievement '{_achievementInfo?.Name}' Progress: {e.Progress}/{e.ProgressMax}");
        }

        Progress = e.Progress;
      }
    }

    private void Trigger_OnTrigger(object? sender, EventArgs e)
    {
      AchievementHookManager.RequestProgess(WatchedID);
    }

    public void Dispose()
    {
      AchievementHookManager.OnAchievementProgress -= AchievementHookManager_OnAchievementProgress;
      Trigger?.Dispose();
      Trigger = null;
    }
  }
}
