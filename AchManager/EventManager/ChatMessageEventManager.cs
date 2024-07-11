using ECommons.DalamudServices;

namespace AchManager.EventManager
{
  internal class ChatMessageEventManager : AchievementUpdateEventManagerBase<ChatMessageEventArgs>
  {
    #region Singleton

    public static ChatMessageEventManager Instance
    {
      get
      {
        _instance ??= new ChatMessageEventManager();
        return _instance;
      }
    }
    private static ChatMessageEventManager? _instance;

    #endregion Singleton

    #region Construction

    private ChatMessageEventManager() 
    {
      Svc.Chat.ChatMessage += Chat_ChatMessage;
    }

    #endregion Construction

    public override void Dispose()
    {
      Svc.Chat.ChatMessage -= Chat_ChatMessage;
    }

    private void Chat_ChatMessage(Dalamud.Game.Text.XivChatType type, int timestamp, ref Dalamud.Game.Text.SeStringHandling.SeString sender, ref Dalamud.Game.Text.SeStringHandling.SeString message, ref bool isHandled)
    {
      Svc.Log.Debug($"{nameof(ChatMessageEventManager)}: Fire");
      FireOnEvent(new ChatMessageEventArgs(message.TextValue));
    }
  }
}
