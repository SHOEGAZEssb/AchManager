using AchManager.EventManager;
using System;

namespace AchManager.AchievementTrigger
{
  [Serializable]
  public class FateCompletedTrigger : AchievementUpdateTriggerBase
  {
    public override string TriggerIdentifier => nameof(FateCompletedTrigger);

    #region Construction

    public FateCompletedTrigger()
    {
      FateCompletedEventManager.Instance.OnEvent += Instance_OnTrigger;
    }

    #endregion Construction

    public override void Dispose()
    {
      FateCompletedEventManager.Instance.OnEvent -= Instance_OnTrigger;
    }

    private void Instance_OnTrigger(object? sender, EventArgs e)
    {
      FireOnTrigger();
    }
  }
}
