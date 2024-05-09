using System;

namespace AchManager.AchievementTrigger
{
  public enum Rank
  {
    All,
    C,
    B,
    A,
    S
  }

  [Serializable]
  public class MarkKilledTriggerConfig : TriggerConfig
  {
    #region Properties

    public Rank RequiredRank { get; set; } = Rank.All;

    #endregion Properties
  }
}
