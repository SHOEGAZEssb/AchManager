using AchManager.EventManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchManager.AchievementTrigger
{
  [Serializable]
  internal class ChatMessageTrigger : AchievementUpdateTriggerBase, IConfigurableTrigger
  {
    #region Properties

    public override string TriggerIdentifier => nameof(ChatMessageTrigger);

    public ITriggerConfig Config => TypedConfig;

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
      if (!string.IsNullOrEmpty(TypedConfig.RequiredMessageContent) &&
        e.Message.Contains(TypedConfig.RequiredMessageContent, StringComparison.CurrentCultureIgnoreCase))
        FireOnTrigger();
    }
  }
}
