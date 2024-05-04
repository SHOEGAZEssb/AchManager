using AchManager.EventManager;
using System;

namespace AchManager.AchievementTrigger
{
  [Serializable]
  public class FateCompletedTrigger : AchievementUpdateTriggerBase
  {
    public override string TriggerIdentifier => nameof(FateCompletedTrigger);

    public override void Dispose()
    {
      FateCompletedEventManager.Instance.OnEvent -= Instance_OnTrigger;
      GC.SuppressFinalize(this);
    }

    protected override void Init()
    {
      FateCompletedEventManager.Instance.OnEvent += Instance_OnTrigger;
    }

    private void Instance_OnTrigger(object? sender, EventArgs e)
    {
      FireOnTrigger();
    }
  }
}
