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

    #region Construction

    private DutyCompletedEventManager()
    {
      Svc.DutyState.DutyCompleted += DutyState_DutyCompleted;
    }

    #endregion Construction

    public override void Dispose()
    {
      Svc.DutyState.DutyCompleted -= DutyState_DutyCompleted;
    }

    private void DutyState_DutyCompleted(object? sender, ushort e)
    {
      Svc.Log.Debug($"{nameof(DutyCompletedEventManager)}: Fire");
      FireOnEvent(EventArgs.Empty);
    }
  }
}
