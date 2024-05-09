using AchManager.AchievementTrigger;
using AchManager.EventManager;
using Dalamud.Configuration;
using Dalamud.Plugin;
using ECommons.DalamudServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

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
  internal Dictionary<uint, AchievementUpdateTriggerBase?> WatchedAchievements { get; set; } = [];

  // the below exist just to make saving less cumbersome
  [NonSerialized]
  private DalamudPluginInterface? PluginInterface;

  [NonSerialized]
  private WatchedAchievementManager? _achievementManager;

  [NonSerialized]
  private static readonly Dictionary<TriggerType, Type?> _availableTrigger = new()
  {
    { TriggerType.DutyCompleted, typeof(DutyCompletedTrigger) },
    { TriggerType.FateCompleted, typeof(FateCompletedTrigger) },
    { TriggerType.MarkKilled, typeof(MarkKilledTrigger) },
    { TriggerType.ChatMessage, typeof(ChatMessageTrigger) },
    { TriggerType.None, null }
  };

  public void Initialize(DalamudPluginInterface pluginInterface)
  {
    PluginInterface = pluginInterface;
    _achievementManager = new WatchedAchievementManager();
    InitializeManager();
  }

  public void Save()
  {
    PluginInterface!.SavePluginConfig(this);

    try
    {
      var serialized = JsonConvert.SerializeObject(WatchedAchievements);
      File.WriteAllText(Path.Combine(PluginInterface!.ConfigDirectory.FullName, "AchManagerWA.json"), serialized);
    }
    catch (Exception ex)
    {
      Svc.Log.Error($"Error while serializing WatchedAchievements: {ex.Message}");
    }
  }

  public void AddWatchedAchievement(uint id)
  {
    if (WatchedAchievements.TryAdd(id, null))
      _achievementManager!.AddWatchedAchievement(id, null);

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
      var type = _availableTrigger[triggerType];
      var trigger = type == null ? null : (AchievementUpdateTriggerBase?)Activator.CreateInstance(type);
      WatchedAchievements[id] = trigger;
      _achievementManager!.SetTriggerTypeForWatchedAchievement(id, trigger);
      Save();
    }
  }

  private void InitializeManager()
  {
    try
    {
      var file = Path.Combine(PluginInterface!.ConfigDirectory.FullName, "AchManagerWA.json");
      if (File.Exists(file))
        WatchedAchievements = JsonConvert.DeserializeObject<Dictionary<uint, AchievementUpdateTriggerBase?>>(File.ReadAllText(file)) ?? [];
    }
    catch (Exception ex)
    {
      Svc.Log.Error($"Error while deserializing WatchedAchievements: {ex.Message}");
    }

    foreach (var ach in WatchedAchievements)
      _achievementManager!.AddWatchedAchievement(ach.Key, ach.Value);
  }
}
