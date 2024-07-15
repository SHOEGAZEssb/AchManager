namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// The quest type to complete for the trigger to trigger.
  /// </summary>
  public enum RequiredQuestType
  {
    /// <summary>
    /// All quest types.
    /// </summary>
    Any = -1,

    /// <summary>
    /// Normal quests.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Levequests.
    /// </summary>
    Leve = 1,

    /// <summary>
    /// Daily quests (beast tribe etc.).
    /// </summary>
    Daily = 2
  }

  /// <summary>
  /// Configuration for a <see cref="QuestCompletedTrigger"/>.
  /// </summary>
  internal class QuestCompletedTriggerConfig : TriggerConfig
  {
    /// <summary>
    /// The quest type to complete for the trigger to trigger.
    /// </summary>
    public RequiredQuestType RequiredQuestType { get; set; } = RequiredQuestType.Any;
  }
}