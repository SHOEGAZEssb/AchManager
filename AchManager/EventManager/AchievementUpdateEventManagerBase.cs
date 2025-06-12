using System;

namespace AchManager.EventManager
{
  public enum TriggerType
  {
    None,
    DutyCompleted,
    FateCompleted,
    MarkKilled,
    ChatMessage,
    QuestCompleded,
    BannerShown
  }

  /// <summary>
  /// Base class for all event managers.
  /// </summary>
  /// <typeparam name="T">EventArgs type.</typeparam>
  internal abstract class AchievementUpdateEventManagerBase<T> : IDisposable where T : EventArgs
  {
    #region Properties

    /// <summary>
    /// Event that is fired when this manager should inform
    /// subscribers that a certain event happened.
    /// </summary>
    public event EventHandler<T>? OnEvent;

    #endregion Properties

    /// <summary>
    /// Disposes this event manager.
    /// </summary>
    public abstract void Dispose();

    /// <summary>
    /// Fires the <see cref="OnEvent"/> event.
    /// </summary>
    /// <param name="eventArgs">Event args.</param>
    protected void FireOnEvent(T eventArgs)
    {
      OnEvent?.Invoke(this, eventArgs);
    }
  }
}