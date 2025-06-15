using System;

namespace AchManager.EventManager
{
  /// <summary>
  /// Event args for the <see cref="ChatMessageEventManager"/>.
  /// </summary>
  /// <param name="message">Chat message content.</param>
  internal sealed class ChatMessageEventArgs(string message) : EventArgs
  {
    #region Properties

    /// <summary>
    /// Chat message content.
    /// </summary>
    public string Message { get; } = message;

    #endregion Properties
  }
}