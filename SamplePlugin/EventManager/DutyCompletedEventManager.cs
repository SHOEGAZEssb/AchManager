using ECommons.DalamudServices;
using System;

namespace AchManager.EventManager
{
  /// <summary>
  /// Trigger that gets fired when a duty successfully completes.
  /// </summary>
  internal class DutyCompletedEventManager : AchievementUpdateEventManagerBase<EventArgs>
  {
    #region Singleton

    public static DutyCompletedEventManager Instance
    {
      get
      {
        _instance ??= new DutyCompletedEventManager();
        return _instance;
      }
    }
    private static DutyCompletedEventManager? _instance;

    #endregion Singleton

    public DutyCompletedEventManager()
    {
      Svc.DutyState.DutyCompleted += DutyState_DutyCompleted;
    }

    private void DutyState_DutyCompleted(object? sender, ushort e)
    {
      FireOnEvent(EventArgs.Empty);
    }
  }
}
