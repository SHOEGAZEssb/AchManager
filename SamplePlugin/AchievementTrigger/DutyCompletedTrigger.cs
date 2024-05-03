using ECommons.DalamudServices;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Trigger that gets fired when a duty successfully completes.
  /// </summary>
  internal class DutyCompletedTrigger : AchievementUpdateTrigger
  {
    public DutyCompletedTrigger()
    {
      Svc.DutyState.DutyCompleted += DutyState_DutyCompleted;
    }

    private void DutyState_DutyCompleted(object? sender, ushort e)
    {
      FireOnTrigger();
    }
  }
}
