using AchManager.AchievementTrigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchManager
{
  internal class WatchedAchievementManager
  {
    private static readonly Dictionary<TriggerType, AchievementUpdateTrigger?> _availableTrigger = new Dictionary<TriggerType, AchievementUpdateTrigger?>()
    {
      { TriggerType.DutyCompleted, new DutyCompletedTrigger() },
      { TriggerType.FateCompleted, new FateTrigger() },
      { TriggerType.MarkKilled, new MarkKilledTrigger() },
      { TriggerType.None, null }
    };

    private readonly List<WatchedAchievement> _achievements = [];

    public void AddWatchedAchievement(uint id, TriggerType triggerType)
    {
      if (_achievements.Any(a => a.WatchedID == id))
        throw new ArgumentException($"Achievement with id {id} is already being watched");

      _achievements.Add(new WatchedAchievement(id, _availableTrigger[triggerType]));
    }

    public void RemoveWatchedAchievement(uint id) 
    {
      var ach = _achievements.FirstOrDefault(a => a.WatchedID == id) ?? throw new ArgumentException($"Can't remove watched achievement: Achievement with id {id} is not being watched");
      ach.Trigger = null; // todo: dispose?
      _achievements.Remove(ach);
    }

    public void SetTriggerTypeForWatchedAchievement(uint id, TriggerType type)
    {
      var ach = _achievements.FirstOrDefault(a => a.WatchedID == id) ?? throw new ArgumentException($"Can't switch trigger type: Achievement with id {id} is not being watched");
      ach.Trigger = _availableTrigger[type];
    }
  }
}
