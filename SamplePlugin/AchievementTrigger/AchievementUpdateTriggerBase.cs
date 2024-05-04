using JsonSubTypes;
using Newtonsoft.Json;
using System;

namespace AchManager.AchievementTrigger
{
  [JsonConverter(typeof(JsonSubtypes), "TriggerIdentifier")]
  [JsonSubtypes.KnownSubType(typeof(FateCompletedTrigger), nameof(FateCompletedTrigger))]
  [JsonSubtypes.KnownSubType(typeof(DutyCompletedTrigger), nameof(DutyCompletedTrigger))]
  [JsonSubtypes.KnownSubType(typeof(MarkKilledTrigger), nameof(MarkKilledTrigger))]
  public abstract class AchievementUpdateTriggerBase : IDisposable
  {
    #region Properties

    public event EventHandler? OnTrigger;

    public abstract string TriggerIdentifier { get; }

    #endregion Properties

    public abstract void Dispose();

    protected void FireOnTrigger()
    {
      OnTrigger?.Invoke(this, EventArgs.Empty);
    }
  }
}
