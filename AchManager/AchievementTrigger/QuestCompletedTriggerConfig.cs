namespace AchManager.AchievementTrigger
{
  public enum RequiredQuestType
  {
    Any = -1,
    Normal = 0,
    Leve = 1,
    Daily = 2
  }

  internal class QuestCompletedTriggerConfig : TriggerConfig
  {
    public RequiredQuestType RequiredQuestType { get; set; } = RequiredQuestType.Any;
  }
}
