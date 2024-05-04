using System;
using System.Collections.Generic;

namespace AchManager.EventManager
{
  public enum KilledEnemyRank
  {
    C,
    B,
    A,
    S,
  }

  internal class MarkKilledEventArgs(KilledEnemyRank rank) : EventArgs
  {
    #region Properties

    public KilledEnemyRank Rank { get; } = rank;

    private static readonly Dictionary<byte, KilledEnemyRank> _rankBytes = new()
    {
      { 0, KilledEnemyRank.C },
      { 1, KilledEnemyRank.B },
      { 2, KilledEnemyRank.A },
      { 3, KilledEnemyRank.S },
      { 4, KilledEnemyRank.S }
    };

    #endregion Properties

    #region Construction

    public MarkKilledEventArgs(byte rank)
      : this(_rankBytes[rank])
    { }

    #endregion Construction
  }
}
