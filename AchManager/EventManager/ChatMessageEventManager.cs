using ECommons.DalamudServices;

namespace AchManager.EventManager
{
    /// <summary>
    /// Event manager that informs about new chat messages.
    /// </summary>
    internal sealed class ChatMessageEventManager : AchievementUpdateEventManagerBase<ChatMessageEventArgs>
    {
        #region Singleton

        /// <summary>
        /// The unique instance of this event manager.
        /// </summary>
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Dispose()
        {
            Svc.Chat.ChatMessage -= Chat_ChatMessage;
        }

        private void Chat_ChatMessage(Dalamud.Game.Chat.IHandleableChatMessage message)
        {
            if (!Plugin.Configuration.PreventChatEventManagerLogSpam)
                Svc.Log.Debug($"{nameof(ChatMessageEventManager)}: Fire");

            FireOnEvent(new ChatMessageEventArgs(message.Message.TextValue));
        }
    }
}