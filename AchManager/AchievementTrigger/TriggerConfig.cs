using System;

namespace AchManager.AchievementTrigger
{
  [Serializable]
  public class TriggerConfig
  {
    #region Properties

    /// <summary>
    /// If true, shows a dalamud notification
    /// when an achievement gains progress.
    /// </summary>
    public bool ShowNotification { get; set; } = true;

    /// <summary>
    /// If true, shows a chat message
    /// when an achievement gains progress.
    /// </summary>
    public bool ShowChatMessage { get; set; } = true;

    /// <summary>
    /// Delay in ms to wait before sending the trigger.
    /// </summary>
    public int DelayMS { get; set; } = 0;

    /// <summary>
    /// If true, triggers only after this trigger has been triggered
    /// <see cref="TriggerEveryCount"/> times.
    /// </summary>
    public bool TriggerEveryXTimes { get; set; } = false;

    /// <summary>
    /// Amount of triggers necessary to actually trigger.
    /// </summary>
    public int TriggerEveryCount { get; set; } = 2;

    #endregion Properties
  }
}