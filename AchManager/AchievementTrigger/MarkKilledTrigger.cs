using AchManager.EventManager;
using System;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Trigger that fires when a mark is killed.
  /// </summary>
  [Serializable]
  public class MarkKilledTrigger : AchievementUpdateTriggerBase
  {
    #region Properties

    public override string TriggerIdentifier => nameof(MarkKilledTrigger);

    public override TriggerConfig Config => TypedConfig;    

    /// <summary>
    /// Configuration for this trigger.
    /// </summary>
    public MarkKilledTriggerConfig TypedConfig { get; } = new MarkKilledTriggerConfig();

    #endregion Properties

    /// <summary>
    /// Disposes this trigger.
    /// Unlinks from the <see cref="MarkKilledEventManager"/>.
    /// </summary>
    public override void Dispose()
    {
      MarkKilledEventManager.Instance.OnEvent -= Instance_OnTrigger;
      GC.SuppressFinalize(this);
    }

    protected override void Init()
    {
      if (_isInitialized)
        return;

      MarkKilledEventManager.Instance.OnEvent += Instance_OnTrigger;
      _isInitialized = true;
    }

    private void Instance_OnTrigger(object? sender, MarkKilledEventArgs e)
    {
      if (TypedConfig.RequiredRank == Rank.All || e.Rank.ToString() == TypedConfig.RequiredRank.ToString())
        FireOnTrigger();
    }
  }
}
