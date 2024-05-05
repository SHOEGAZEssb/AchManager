using System;

namespace AchManager.EventManager
{
    public enum TriggerType
    {
        None,
        DutyCompleted,
        FateCompleted,
        MarkKilled,
        ChatMessage
    }

    internal abstract class AchievementUpdateEventManagerBase<T> where T : EventArgs
    {
        public event EventHandler<T>? OnEvent;

        protected void FireOnEvent(T eventArgs)
        {
            OnEvent?.Invoke(this, eventArgs);
        }
    }
}
