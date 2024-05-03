using AchManager.AchievementTrigger;
using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace AchManager;

[Serializable]
public class Configuration : IPluginConfiguration
{
  public int Version { get; set; } = 0;

  /// <summary>
  /// Holds the ids of all watched achievements together with the
  /// configured trigger type. Should not be modified directly.
  /// Use <see cref="AddWatchedAchievement(uint)"/> or <see cref="RemoveWatchedAchievement(uint)"/>.
  /// </summary>
  public Dictionary<uint, TriggerType> WatchedAchievements { get; set; } = [];

  // the below exist just to make saving less cumbersome
  [NonSerialized]
  private DalamudPluginInterface? PluginInterface;

  [NonSerialized]
  private WatchedAchievementManager? _achievementManager;

  public void Initialize(DalamudPluginInterface pluginInterface)
  {
    PluginInterface = pluginInterface;
    PluginInterface.GetPluginConfig();
    _achievementManager = new WatchedAchievementManager();
    InitializeManager();
  }

  public void Save()
  {
    PluginInterface!.SavePluginConfig(this);
  }

  public void AddWatchedAchievement(uint id)
  {
    if (WatchedAchievements.TryAdd(id, TriggerType.None)) 
      _achievementManager!.AddWatchedAchievement(id, TriggerType.None);

    Save();
  }

  public void RemoveWatchedAchievement(uint id) 
  {
    if (WatchedAchievements.Remove(id))
      _achievementManager!.RemoveWatchedAchievement(id);

    Save();
  }

  public void ChangeTriggerTypeForAchievement(uint id, TriggerType triggerType)
  {
    if (WatchedAchievements.ContainsKey(id))
    {
      WatchedAchievements[id] = triggerType;
      _achievementManager!.SetTriggerTypeForWatchedAchievement(id, triggerType);
      Save();
    }
  }

  private void InitializeManager()
  {
    foreach (var ach in WatchedAchievements)
      _achievementManager!.AddWatchedAchievement(ach.Key, ach.Value);
  }
}
