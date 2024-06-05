using AchManager.AchievementTrigger;
using Dalamud.Interface.ImGuiNotification;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AchManager
{
  internal class WatchedAchievement : IDisposable
  {
    /// <summary>
    /// Event that is fired when the achievement gets completed.
    /// </summary>
    public event EventHandler? OnCompleted;

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

    public uint? Progress { get; private set; }
    public uint? ProgressMax { get; private set; }

    public uint WatchedID { get; private set; }
    private readonly Lumina.Excel.GeneratedSheets.Achievement? _achievementInfo;

    private readonly List<IActiveNotification> _activeNotifications = [];

    public WatchedAchievement(uint id, AchievementUpdateTriggerBase? trigger)
    {
      AchievementHookManager.OnAchievementProgress += AchievementHookManager_OnAchievementProgress;
      WatchedID = id;
      Trigger = trigger;
      _achievementInfo = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Achievement>()?.FirstOrDefault(a => a.RowId == WatchedID);

      // get initial progress
      AchievementHookManager.RequestProgess(WatchedID);
    }

    private void AchievementHookManager_OnAchievementProgress(object? sender, AchievementProgressEventArgs e)
    {
      try
      {
        if (e.ID == WatchedID)
        {
          if (Progress != e.Progress)
          {
            if (Progress != null)
            {
              if (Trigger?.Config.ShowChatMessage ?? false)
                Svc.Chat.Print($"Achievement '{_achievementInfo?.Name}' Progress: {e.Progress}/{e.ProgressMax}");

              if (Trigger?.Config.ShowNotification ?? false)
              {
                var notif = new Notification
                {
                  InitialDuration = TimeSpan.FromSeconds(3),
                  Title = _achievementInfo?.Name ?? string.Empty,
                  Type = Dalamud.Interface.Internal.Notifications.NotificationType.Success,
                  Content = $"{_achievementInfo?.Name}:\n{e.Progress}/{e.ProgressMax}",
                  Progress = e.Progress / e.ProgressMax,
                  IconTexture = Plugin.TextureProvider.GetIcon(_achievementInfo?.Icon ?? 0)
                };

                var newNotif = Svc.NotificationManager.AddNotification(notif);
                newNotif.Dismiss += LastNotification_Dismiss;
                _activeNotifications.Add(newNotif);
              }
            }
          }

          Progress = e.Progress;
          ProgressMax = e.ProgressMax;

          if (Progress == ProgressMax)
            OnCompleted?.Invoke(this, e);
        }
      }
      catch (Exception ex)
      {
        Svc.Log.Error($"Error getting / updating achievement progress for {WatchedID}: {ex.Message}");
      }
    }

    private void LastNotification_Dismiss(Dalamud.Interface.ImGuiNotification.EventArgs.INotificationDismissArgs obj)
    {
      obj.Notification.Dismiss -= LastNotification_Dismiss;
      obj.Notification.IconTexture?.Dispose();
      _activeNotifications.Remove(obj.Notification);
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
