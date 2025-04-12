using ECommons.DalamudServices;

namespace AchManager.EventManager
{
  /// <summary>
  /// Event manager that informs about when a duty successfully completes.
  /// </summary>
  internal class DutyCompletedEventManager : AchievementUpdateEventManagerBase<DutyCompletedEventArgs>
  {
    #region Singleton

    /// <summary>
    /// The unique instance of this event manager.
    /// </summary>
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Dispose()
    {
      Svc.DutyState.DutyCompleted -= DutyState_DutyCompleted;
    }

    private void DutyState_DutyCompleted(object? sender, ushort e)
    {
      Svc.Log.Debug($"{nameof(DutyCompletedEventManager)}: Fire ({e})");
      FireOnEvent(new DutyCompletedEventArgs(e));
    }
  }
}