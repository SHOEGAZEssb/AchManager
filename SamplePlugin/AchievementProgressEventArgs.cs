using System;

namespace AchManager
{
  internal class AchievementProgressEventArgs(uint id, uint progress, uint max) : EventArgs
  {
    #region Properties

    public uint ID { get; } = id;
    public uint Progress { get; } = progress;
    public uint ProgressMax { get; } = max;

    #endregion Properties
  }
}