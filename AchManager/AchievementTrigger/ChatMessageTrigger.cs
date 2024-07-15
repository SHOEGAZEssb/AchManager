using AchManager.EventManager;
using System;
using System.Text.RegularExpressions;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Trigger that triggers when a chat message fullfills certain conditions.
  /// </summary>
  [Serializable]
  internal class ChatMessageTrigger : AchievementUpdateTriggerBase
  {
    #region Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string TriggerIdentifier => nameof(ChatMessageTrigger);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override TriggerConfig Config => TypedConfig;

    /// <summary>
    /// The specific configuration for this trigger.
    /// </summary>
    public ChatMessageTriggerConfig TypedConfig { get; } = new ChatMessageTriggerConfig();

    #endregion Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Dispose()
    {
      ChatMessageEventManager.Instance.OnEvent -= ChatMessageEventManager_OnEvent;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void Init()
    {
      if (_isInitialized)
        return;

      ChatMessageEventManager.Instance.OnEvent += ChatMessageEventManager_OnEvent;
      _isInitialized = true;
    }

    private void ChatMessageEventManager_OnEvent(object? sender, ChatMessageEventArgs e)
    {
      if (string.IsNullOrEmpty(TypedConfig.RequiredMessageContent) || string.IsNullOrEmpty(e.Message))
        return;

      if (TypedConfig.IsRegex && Regex.IsMatch(e.Message, TypedConfig.RequiredMessageContent))
        FireOnTrigger();
      else if (e.Message.Contains(TypedConfig.RequiredMessageContent, StringComparison.CurrentCultureIgnoreCase))
        FireOnTrigger();        
    }
  }
}
