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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string TriggerIdentifier => nameof(MarkKilledTrigger);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override TriggerConfig Config => TypedConfig;    

    /// <summary>
    /// The specific configuration for this trigger.
    /// </summary>
    public MarkKilledTriggerConfig TypedConfig { get; } = new MarkKilledTriggerConfig();

    #endregion Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Dispose()
    {
      MarkKilledEventManager.Instance.OnEvent -= MarkKilledEventManager_OnEvent;
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void Init()
    {
      if (_isInitialized)
        return;

      MarkKilledEventManager.Instance.OnEvent += MarkKilledEventManager_OnEvent;
      _isInitialized = true;
    }

    private void MarkKilledEventManager_OnEvent(object? sender, MarkKilledEventArgs e)
    {
      if (TypedConfig.RequiredRank == Rank.All || e.Rank.ToString() == TypedConfig.RequiredRank.ToString())
        FireOnTrigger();
    }
  }
}
