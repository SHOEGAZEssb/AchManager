using AchManager.EventManager;
using System;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Trigger that triggers when a fate has been completed.
  /// </summary>
  [Serializable]
  public class FateCompletedTrigger : AchievementUpdateTriggerBase
  {
    #region Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string TriggerIdentifier => nameof(FateCompletedTrigger);

    #endregion Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Dispose()
    {
      FateCompletedEventManager.Instance.OnEvent -= FateCompletedEventManager_OnEvent;
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void Init()
    {
      if (_isInitialized) 
        return;

      FateCompletedEventManager.Instance.OnEvent += FateCompletedEventManager_OnEvent;
      _isInitialized = true;
    }

    private void FateCompletedEventManager_OnEvent(object? sender, EventArgs e)
    {
      FireOnTrigger();
    }
  }
}
