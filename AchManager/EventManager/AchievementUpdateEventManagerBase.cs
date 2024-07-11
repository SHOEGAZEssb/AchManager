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
    QuestCompleded
  }

  internal abstract class AchievementUpdateEventManagerBase<T> : IDisposable where T : EventArgs
  {
    public event EventHandler<T>? OnEvent;

    public abstract void Dispose();

    protected void FireOnEvent(T eventArgs)
    {
      OnEvent?.Invoke(this, eventArgs);
    }
  }
}
