using ECommons.Automation.NeoTaskManager;
using ECommons.GameHelpers;
using JsonSubTypes;
using Newtonsoft.Json;
using System;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Base class for all achievement triggers.
  /// </summary>
  [JsonConverter(typeof(JsonSubtypes), nameof(TriggerIdentifier))]
  [JsonSubtypes.KnownSubType(typeof(FateCompletedTrigger), nameof(FateCompletedTrigger))]
  [JsonSubtypes.KnownSubType(typeof(DutyCompletedTrigger), nameof(DutyCompletedTrigger))]
  [JsonSubtypes.KnownSubType(typeof(MarkKilledTrigger), nameof(MarkKilledTrigger))]
  [JsonSubtypes.KnownSubType(typeof(ChatMessageTrigger), nameof(ChatMessageTrigger))]
  [JsonSubtypes.KnownSubType(typeof(QuestCompletedTrigger), nameof(QuestCompletedTrigger))]
  public abstract class AchievementUpdateTriggerBase : IDisposable
  {
    #region Properties

    /// <summary>
    /// Event that is fired when this trigger triggers.
    /// </summary>
    public event EventHandler? OnTrigger;

    /// <summary>
    /// Identifier of this trigger for deserializing.
    /// </summary>
    public abstract string TriggerIdentifier { get; }

    /// <summary>
    /// The configuration of this trigger.
    /// </summary>
    public virtual TriggerConfig Config { get; }

    /// <summary>
    /// Safety bool to stop double initialization.
    /// </summary>
    protected bool _isInitialized;

    private readonly TaskManager _taskManager = new();

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    protected AchievementUpdateTriggerBase()
    {
      Config = new TriggerConfig();
      Init();
    }

    #endregion Construction

    /// <summary>
    /// Disposes this trigger.
    /// </summary>
    public abstract void Dispose();

    /// <summary>
    /// Fires this trigger if requirements are met.
    /// </summary>
    protected void FireOnTrigger()
    {
      if (CanFire())
      {
        _taskManager.EnqueueDelay(Config.DelayMS);
        _taskManager.Enqueue(() =>
        {
          OnTrigger?.Invoke(this, EventArgs.Empty);
        });
      }
    }

    /// <summary>
    /// Initializes this trigger.
    /// </summary>
    protected abstract void Init();

    /// <summary>
    /// Checks if the trigger can currently trigger
    /// based on its configuration.
    /// </summary>
    /// <returns>True if the trigger can trigger, otherwise false.</returns>
    private bool CanFire()
    {
      var jobMatches = Config.RequiredJob == Job.Any || (int)Player.Job == (int)Config.RequiredJob;
      var noTerritoryRestriction = Config.RequiredTerritories.Count == 0;
      var territoryMatchesWhitelist = Config.TreatRequiredTerritoriesAsWhitelist && Config.RequiredTerritories.Contains(Player.Territory);
      var territoryMatchesBlacklist = !Config.TreatRequiredTerritoriesAsWhitelist && !Config.RequiredTerritories.Contains(Player.Territory);

      return jobMatches && (noTerritoryRestriction || territoryMatchesWhitelist || territoryMatchesBlacklist);
    }
  }
}