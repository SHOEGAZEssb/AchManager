using System;

namespace AchManager.EventManager
{
  /// <summary>
  /// EventArgs for the <see cref="DutyCompletedEventManager"/>.
  /// </summary>
  /// <param name="territoryID"></param>
  public sealed class DutyCompletedEventArgs(ushort territoryID) : EventArgs
  {
    #region Properties

    /// <summary>
    /// Territory ID of the duty.
    /// </summary>
    public ushort TerritoryID { get; } = territoryID;

    #endregion Properties
  }
}