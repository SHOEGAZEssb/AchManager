using System;

namespace AchManager.AchievementTrigger
{
  public enum TriggerType
  {
    None,
    DutyCompleted,
    FateCompleted,
    MarkKilled
  }

  internal abstract class AchievementUpdateTrigger
  {
    public event EventHandler? OnTrigger;

    protected void FireOnTrigger()
    {
      OnTrigger?.Invoke(this, EventArgs.Empty);
    }
  }
}
