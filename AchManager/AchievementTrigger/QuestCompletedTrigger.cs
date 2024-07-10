using AchManager.EventManager;

namespace AchManager.AchievementTrigger
{
  internal class QuestCompletedTrigger : AchievementUpdateTriggerBase
  {
    #region Properties

    public override string TriggerIdentifier => nameof(QuestCompletedTrigger);

    public override TriggerConfig Config => TypedConfig;

    /// <summary>
    /// Configuration for this trigger.
    /// </summary>
    public QuestCompletedTriggerConfig TypedConfig { get; } = new QuestCompletedTriggerConfig();

    #endregion Properties

    public override void Dispose()
    {
      QuestCompletedEventManager.Instance.OnEvent -= QuestCompletedEventManager_OnEvent;
    }

    protected override void Init()
    {
      if (_isInitialized)
        return;

      QuestCompletedEventManager.Instance.OnEvent += QuestCompletedEventManager_OnEvent;
      _isInitialized = true;
    }

    private void QuestCompletedEventManager_OnEvent(object? sender, QuestCompletedEventArgs e)
    {
      var questType = (RequiredQuestType)e.CompletedQuestType;
      if (TypedConfig.RequiredQuestType == RequiredQuestType.Any || questType.HasFlag(TypedConfig.RequiredQuestType))
        FireOnTrigger();
    }
  }
}
