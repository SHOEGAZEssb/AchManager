using System;

namespace AchManager.EventManager
{
  public enum KilledEnemyRank
  {
    C,
    B,
    A,
    S
  }

  internal class MarkKilledEventArgs(KilledEnemyRank rank) : EventArgs
  {
    #region Properties

    public KilledEnemyRank Rank { get; } = rank;

    #endregion Properties
  }
}
