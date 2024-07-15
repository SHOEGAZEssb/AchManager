using System;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Configuration for a <see cref="ChatMessageTrigger"/>.
  /// </summary>
  [Serializable]
  internal class ChatMessageTriggerConfig() : TriggerConfig
  {
    #region Properties

    /// <summary>
    /// The required content of a chat message.
    /// </summary>
    public string RequiredMessageContent { get; set; } = string.Empty;

    /// <summary>
    /// If the <see cref="RequiredMessageContent"/> is a Regex string.
    /// </summary>
    public bool IsRegex { get; set; } = false;

    #endregion Properties
  }
}