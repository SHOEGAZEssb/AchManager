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

    #endregion Properties
  }
}