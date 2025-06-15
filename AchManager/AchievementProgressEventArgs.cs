using System;

namespace AchManager
{
  /// <summary>
  /// EventArgs for achievement progress.
  /// </summary>
  /// <param name="id">ID of the achievement.</param>
  /// <param name="progress">New progress of the achievement.</param>
  /// <param name="max">Maximum progress of the achievement.</param>
  internal sealed class AchievementProgressEventArgs(uint id, uint progress, uint max) : EventArgs
  {
    #region Properties

    /// <summary>
    /// ID of the achievement.
    /// </summary>
    public uint ID { get; } = id;

    /// <summary>
    /// New progress of the achievement.
    /// </summary>
    public uint Progress { get; } = progress;

    /// <summary>
    /// Maximum progress of the achievement.
    /// </summary>
    public uint ProgressMax { get; } = max;

    #endregion Properties
  }
}