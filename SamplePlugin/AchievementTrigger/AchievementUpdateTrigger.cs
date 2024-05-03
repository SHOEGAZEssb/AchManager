using System;

namespace AchManager.AchievementTrigger
{
  internal abstract class AchievementUpdateTrigger
  {
    public event EventHandler? OnTrigger;

    protected void FireOnTrigger()
    {
      OnTrigger?.Invoke(this, EventArgs.Empty);
    }
  }
}
