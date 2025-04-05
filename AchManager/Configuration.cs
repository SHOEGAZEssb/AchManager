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

  public bool PreventChatEventManagerLogSpam { get; set; } = true;

  /// <summary>
  /// List of watched achievements.
  /// </summary>
  [JsonIgnore]
  public IEnumerable<WatchedAchievement> Achievements => _achievementManager?.Achievements ?? [];

  /// <summary>
  /// Holds the ids of all watched achievements together with the
  /// configured trigger type. Should not be modified directly.
  /// Use <see cref="AddWatchedAchievement(uint)"/> or <see cref="RemoveWatchedAchievement(uint)"/>.
  /// </summary>
  internal Dictionary<uint, AchievementUpdateTriggerBase?> WatchedAchievements { get; set; } = [];

  // the below exist just to make saving less cumbersome
  [NonSerialized]
  private IDalamudPluginInterface? PluginInterface;

  [NonSerialized]
  private WatchedAchievementManager? _achievementManager;

  [NonSerialized]
  private static readonly Dictionary<TriggerType, Type?> _availableTrigger = new()
  {
    { TriggerType.DutyCompleted, typeof(DutyCompletedTrigger) },
    { TriggerType.FateCompleted, typeof(FateCompletedTrigger) },
    { TriggerType.MarkKilled, typeof(MarkKilledTrigger) },
    { TriggerType.ChatMessage, typeof(ChatMessageTrigger) },
    { TriggerType.QuestCompleded, typeof(QuestCompletedTrigger) },
    { TriggerType.None, null }
  };

  public void Initialize(IDalamudPluginInterface pluginInterface)
  {
    PluginInterface = pluginInterface;
    _achievementManager = new WatchedAchievementManager();
    _achievementManager.OnWatchedAchievementRemovalRequested += AchievementManager_OnWatchedAchievementRemovalRequested;
    InitializeManager();
  }

  public void Save()
  {
    var settings = new JsonSerializerSettings()
    {
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };

    var serializedThis = JsonConvert.SerializeObject(this, settings);
    File.WriteAllText(Path.Combine(PluginInterface!.ConfigDirectory.FullName, "AchManager.json"), serializedThis);

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

  public static Configuration? Load(string path)
  {
    try
    {
      var settings = new JsonSerializerSettings()
      {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
      };

      return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path), settings);
    }
    catch (Exception ex)
    {
      Svc.Log.Error($"Error while loading plugin configuration: {ex.Message}");
      return null;
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
    {
      _achievementManager!.RemoveWatchedAchievement(id);
      Save();
    }
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

  public void FetchProgress()
  {
    foreach (var ach in WatchedAchievements.Keys)
    {
      AchievementHookManager.RequestProgess(ach);
    }

    Svc.Chat.Print("Achievement Progress fetched");
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

  private void AchievementManager_OnWatchedAchievementRemovalRequested(object? sender, EventArgs e)
  {
    if (sender is WatchedAchievement ach)
    {
      RemoveWatchedAchievement(ach.WatchedID);
      Svc.Log.Info($"Achievement with ID {ach.WatchedID} removed from watchlist, due to it being completed.");
    }
  }
}
