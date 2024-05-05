namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Interface for a trigger that is configurable.
  /// </summary>
  internal interface IConfigurableTrigger
  {
    /// <summary>
    /// The configuration of this trigger.
    /// </summary>
    ITriggerConfig Config { get; }
  }
}
