using AchManager.EventManager;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Trigger that triggers when a certain quest has been completed.
  /// </summary>
  internal class QuestCompletedTrigger : AchievementUpdateTriggerBase
  {
    #region Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string TriggerIdentifier => nameof(QuestCompletedTrigger);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override TriggerConfig Config => TypedConfig;

    /// <summary>
    /// The specific configuration for this trigger.
    /// </summary>
    public QuestCompletedTriggerConfig TypedConfig { get; } = new QuestCompletedTriggerConfig();

    #endregion Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Dispose()
    {
      QuestCompletedEventManager.Instance.OnEvent -= QuestCompletedEventManager_OnEvent;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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
