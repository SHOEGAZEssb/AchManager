using System;

namespace AchManager.EventManager
{
  internal class ChatMessageEventArgs(string message) : EventArgs
  {
    #region Properties

    public string Message { get; } = message;

    #endregion Properties
  }
}
