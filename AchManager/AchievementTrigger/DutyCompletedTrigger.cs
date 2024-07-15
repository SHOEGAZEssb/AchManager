using AchManager.EventManager;
using System;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Trigger that triggers when a duty is completed.
  /// </summary>
  [Serializable]
  public class DutyCompletedTrigger : AchievementUpdateTriggerBase
  {
    #region Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string TriggerIdentifier => nameof(DutyCompletedTrigger);

    #endregion Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Dispose()
    {
      DutyCompletedEventManager.Instance.OnEvent -= DutyCompletedEventManager_OnEvent;
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void Init()
    {
      if (_isInitialized)
        return;

      DutyCompletedEventManager.Instance.OnEvent += DutyCompletedEventManager_OnEvent;
      _isInitialized = true;
    }

    private void DutyCompletedEventManager_OnEvent(object? sender, EventArgs e)
    {
      FireOnTrigger();
    }
  }
}