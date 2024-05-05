using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace AchManager.AchievementTrigger
{
  [JsonConverter(typeof(JsonSubtypes), nameof(TriggerIdentifier))]
  [JsonSubtypes.KnownSubType(typeof(FateCompletedTrigger), nameof(FateCompletedTrigger))]
  [JsonSubtypes.KnownSubType(typeof(DutyCompletedTrigger), nameof(DutyCompletedTrigger))]
  [JsonSubtypes.KnownSubType(typeof(MarkKilledTrigger), nameof(MarkKilledTrigger))]
  [JsonSubtypes.KnownSubType(typeof(ChatMessageTrigger), nameof(ChatMessageTrigger))]
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
    /// Safety bool to stop double initialization.
    /// </summary>
    protected bool _isInitialized;

    #endregion Properties

    #region Construction

    protected AchievementUpdateTriggerBase()
    {
      Init();
    }

    #endregion Construction

    public abstract void Dispose();

    protected void FireOnTrigger()
    {
      OnTrigger?.Invoke(this, EventArgs.Empty);
    }

    protected abstract void Init();
  }
}
