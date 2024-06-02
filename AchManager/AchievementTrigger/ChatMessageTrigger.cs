using AchManager.EventManager;
using System;
using System.Text.RegularExpressions;

namespace AchManager.AchievementTrigger
{
  [Serializable]
  internal class ChatMessageTrigger : AchievementUpdateTriggerBase
  {
    #region Properties

    public override string TriggerIdentifier => nameof(ChatMessageTrigger);

    public override TriggerConfig Config => TypedConfig;

    /// <summary>
    /// Configuration for this trigger.
    /// </summary>
    public ChatMessageTriggerConfig TypedConfig { get; } = new ChatMessageTriggerConfig();

    #endregion Properties

    public override void Dispose()
    {
      ChatMessageEventManager.Instance.OnEvent -= ChatMessageEventManager_OnEvent;
    }

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
