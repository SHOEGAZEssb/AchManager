using System;
using System.Collections.Generic;

namespace AchManager.EventManager
{
  /// <summary>
  /// Rank of the killed mark.
  /// </summary>
  public enum KilledEnemyRank
  {
    /// <summary>
    /// C rank.
    /// </summary>
    C,

    /// <summary>
    /// B rank.
    /// </summary>
    B,

    /// <summary>
    /// A rank.
    /// </summary>
    A,

    /// <summary>
    /// S rank.
    /// </summary>
    S,
  }

  /// <summary>
  /// Event args for the <see cref="MarkKilledEventManager"/>.
  /// </summary>
  /// <param name="rank"></param>
  internal sealed class MarkKilledEventArgs(KilledEnemyRank rank) : EventArgs
  {
    #region Properties

    /// <summary>
    /// Rank of the killed mark.
    /// </summary>
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

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="rank">The rank as byte.</param>
    public MarkKilledEventArgs(byte rank)
      : this(_rankBytes[rank])
    { }

    #endregion Construction
  }
}
