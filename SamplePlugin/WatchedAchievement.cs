using AchManager.AchievementTrigger;
using Dalamud.Interface.ImGuiNotification;
using ECommons.DalamudServices;
using System;
using System.Linq;

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

    private IActiveNotification? _lastNotification;

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
          if (Plugin.Configuration.ShowChatMessage)
            Svc.Chat.Print($"Achievement '{_achievementInfo?.Name}' Progress: {e.Progress}/{e.ProgressMax}");

          if (Plugin.Configuration.ShowNotification)
          {
            var notif = new Notification
            {
              InitialDuration = TimeSpan.FromSeconds(3),
              Title = _achievementInfo?.Name ?? string.Empty,
              Type = Dalamud.Interface.Internal.Notifications.NotificationType.Success,
              Content = $"{_achievementInfo?.Name}:\n{e.Progress}/{e.ProgressMax}",
              Progress = e.Progress / e.ProgressMax,
              IconTexture = Plugin.TextureProvider.GetIcon(_achievementInfo.Icon)
            };

            _lastNotification = Svc.NotificationManager.AddNotification(notif);
            _lastNotification.Dismiss += LastNotification_Dismiss;
          }
        }

        Progress = e.Progress;
      }
    }

    private void LastNotification_Dismiss(Dalamud.Interface.ImGuiNotification.EventArgs.INotificationDismissArgs obj)
    {
      _lastNotification!.Dismiss -= LastNotification_Dismiss;
      _lastNotification!.IconTexture?.Dispose();
      _lastNotification = null;
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
