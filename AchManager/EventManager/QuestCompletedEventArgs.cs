using System;

namespace AchManager.EventManager
{
  [Flags]
  public enum CompletedQuestType
  {
    Normal = 0,
    Leve = 1,
    Daily = 2
  }

  internal class QuestCompletedEventArgs(CompletedQuestType completedQuestType) : EventArgs
  {
    public CompletedQuestType CompletedQuestType { get; } = completedQuestType;
  }
}
