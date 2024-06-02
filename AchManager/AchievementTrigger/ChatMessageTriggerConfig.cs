using System;

namespace AchManager.AchievementTrigger
{
  [Serializable]
  internal class ChatMessageTriggerConfig() : TriggerConfig
  {
    #region Properties

    public string RequiredMessageContent { get; set; } = string.Empty;

    public bool IsRegex { get; set; } = false;

    #endregion Properties
  }
}
