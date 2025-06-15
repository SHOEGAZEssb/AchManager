using System;

namespace AchManager.EventManager
{
  /// <summary>
  /// Type of the completed quest.
  /// </summary>
  [Flags]
  public enum CompletedQuestType
  {
    /// <summary>
    /// Normal quest.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Levequest.
    /// </summary>
    Leve = 1,

    /// <summary>
    /// Daily quest (beast tribe etc.).
    /// </summary>
    Daily = 2
  }

  /// <summary>
  /// Event args for the <see cref="QuestCompletedEventManager"/>.
  /// </summary>
  /// <param name="completedQuestType">The type of the completed quest.</param>
  internal sealed class QuestCompletedEventArgs(CompletedQuestType completedQuestType) : EventArgs
  {
    /// <summary>
    /// The type of the completed quest.
    /// </summary>
    public CompletedQuestType CompletedQuestType { get; } = completedQuestType;
  }
}