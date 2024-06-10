using AchManager.AchievementTrigger;
using Dalamud.Interface.ImGuiNotification;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AchManager
{
  public class WatchedAchievement : IDisposable
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
    public Lumina.Excel.GeneratedSheets.Achievement AchievementInfo { get; }

    private readonly List<IActiveNotification> _activeNotifications = [];

    private bool _initialized = false;

    private int _progressSteps = 0;

    public WatchedAchievement(uint id, AchievementUpdateTriggerBase? trigger)
    {
      AchievementHookManager.OnAchievementProgress += AchievementHookManager_OnAchievementProgress;
      WatchedID = id;
      Trigger = trigger;
      AchievementInfo = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Achievement>()?.First(a => a.RowId == WatchedID) ?? throw new ArgumentException($"Could not get achievement info for {WatchedID}");
    }

    /// <summary>
    /// Fetches initial achievement progress.
    /// </summary>
    /// <exception cref="InvalidOperationException">If achievement is already initialized.</exception>
    public void Initialize()
    {
      if (_initialized)
        throw new InvalidOperationException($"Achievement {WatchedID} is already initialized");

      AchievementHookManager.RequestProgess(WatchedID);
      _initialized = true;
    }

    /// <summary>
    /// Resets cached progress.
    /// </summary>
    public void Deinitialize()
    {
      Progress = null;
      ProgressMax = null;
      _initialized = false;
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
              _progressSteps += (int)(e.Progress - Progress);
              if (!Trigger?.Config.TriggerEveryXTimes ?? true || _progressSteps >= Trigger.Config.TriggerEveryCount || e.Progress == e.ProgressMax)
              {
                _progressSteps = 0;

                if (Trigger?.Config.ShowChatMessage ?? false)
                  Svc.Chat.Print($"Achievement '{AchievementInfo?.Name}' Progress: {e.Progress}/{e.ProgressMax}");

                if (Trigger?.Config.ShowNotification ?? false)
                {
                  var notif = new Notification
                  {
                    InitialDuration = TimeSpan.FromSeconds(3),
                    Title = AchievementInfo?.Name ?? string.Empty,
                    Type = Dalamud.Interface.Internal.Notifications.NotificationType.Success,
                    Content = $"{AchievementInfo?.Name}:\n{e.Progress}/{e.ProgressMax}",
                    Progress = e.Progress / e.ProgressMax,
                    IconTexture = Plugin.TextureProvider.GetIcon(AchievementInfo?.Icon ?? 0)
                  };

                  var newNotif = Svc.NotificationManager.AddNotification(notif);
                  newNotif.Dismiss += LastNotification_Dismiss;
                  _activeNotifications.Add(newNotif);
                }
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

      GC.SuppressFinalize(this);
    }
  }
}
