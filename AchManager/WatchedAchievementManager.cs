using AchManager.AchievementTrigger;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AchManager
{
  internal class WatchedAchievementManager
  {
    /// <summary>
    /// Event that is fired when a watched achievement should be removed from any
    /// parent lists holding it.
    /// </summary>
    public event EventHandler? OnWatchedAchievementRemovalRequested;

    private readonly List<WatchedAchievement> _achievements = [];

    public void AddWatchedAchievement(uint id, AchievementUpdateTriggerBase? trigger)
    {
      if (_achievements.Any(a => a.WatchedID == id))
        throw new ArgumentException($"Achievement with id {id} is already being watched");

      var ach = new WatchedAchievement(id, trigger);
      ach.OnCompleted += Ach_OnCompleted;
      _achievements.Add(ach);
    }

    private void Ach_OnCompleted(object? sender, EventArgs e)
    {
      OnWatchedAchievementRemovalRequested?.Invoke(sender, e);
    }

    public void RemoveWatchedAchievement(uint id)
    {
      var ach = _achievements.FirstOrDefault(a => a.WatchedID == id) ?? throw new ArgumentException($"Can't remove watched achievement: Achievement with id {id} is not being watched");
      _achievements.Remove(ach);
      ach.OnCompleted -= Ach_OnCompleted;
      ach.Dispose();
    }

    public void SetTriggerTypeForWatchedAchievement(uint id, AchievementUpdateTriggerBase? trigger)
    {
      var ach = _achievements.FirstOrDefault(a => a.WatchedID == id) ?? throw new ArgumentException($"Can't switch trigger type: Achievement with id {id} is not being watched");
      ach.Trigger = trigger;
    }

    public WatchedAchievement GetAchievement(uint id)
    {
      return _achievements.First(a => a.WatchedID == id);
    }
  }
}
