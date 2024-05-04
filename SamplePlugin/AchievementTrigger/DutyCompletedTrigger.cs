using AchManager.EventManager;
using System;

namespace AchManager.AchievementTrigger
{
  [Serializable]
  public class DutyCompletedTrigger : AchievementUpdateTriggerBase
  {
    public override string TriggerIdentifier => nameof(DutyCompletedTrigger);

    public override void Dispose()
    {
      DutyCompletedEventManager.Instance.OnEvent -= Instance_OnTrigger;
      GC.SuppressFinalize(this);
    }

    protected override void Init()
    {
      if (_isInitialized)
        return;

      DutyCompletedEventManager.Instance.OnEvent += Instance_OnTrigger;
      _isInitialized = true;
    }

    private void Instance_OnTrigger(object? sender, EventArgs e)
    {
      FireOnTrigger();
    }
  }
}
