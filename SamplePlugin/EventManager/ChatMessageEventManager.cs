using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    private ChatMessageEventManager() 
    {
      Svc.Chat.ChatMessage += Chat_ChatMessage;
    }

    private void Chat_ChatMessage(Dalamud.Game.Text.XivChatType type, uint senderId, ref Dalamud.Game.Text.SeStringHandling.SeString sender, ref Dalamud.Game.Text.SeStringHandling.SeString message, ref bool isHandled)
    {
      FireOnEvent(new ChatMessageEventArgs(message.TextValue));
    }
  }
}
