using System;

namespace AchManager.EventManager
{
  /// <summary>
  /// EventArgs for the <see cref="DutyCompletedEventManager"/>.
  /// </summary>
  /// <param name="territoryID"></param>
  public class DutyCompletedEventArgs(ushort territoryID) : EventArgs
  {
    #region Properties

    public ushort TerritoryID { get; } = territoryID;

    #endregion Properties
  }
}