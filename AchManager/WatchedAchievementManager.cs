using AchManager.AchievementTrigger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AchManager
{
  internal class WatchedAchievementManager
  {
    private readonly List<WatchedAchievement> _achievements = [];

    public void AddWatchedAchievement(uint id, AchievementUpdateTriggerBase? trigger)
    {
      if (_achievements.Any(a => a.WatchedID == id))
        throw new ArgumentException($"Achievement with id {id} is already being watched");

      _achievements.Add(new WatchedAchievement(id, trigger));
    }

    public void RemoveWatchedAchievement(uint id)
    {
      var ach = _achievements.FirstOrDefault(a => a.WatchedID == id) ?? throw new ArgumentException($"Can't remove watched achievement: Achievement with id {id} is not being watched");
      _achievements.Remove(ach);
      ach.Dispose();
    }

    public void SetTriggerTypeForWatchedAchievement(uint id, AchievementUpdateTriggerBase? trigger)
    {
      var ach = _achievements.FirstOrDefault(a => a.WatchedID == id) ?? throw new ArgumentException($"Can't switch trigger type: Achievement with id {id} is not being watched");
      ach.Trigger = trigger;
    }
  }
}
