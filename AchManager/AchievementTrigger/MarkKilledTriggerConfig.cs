using System;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Required rank of a killed mark.
  /// </summary>
  public enum Rank
  {
    /// <summary>
    /// All ranks.
    /// </summary>
    All,

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
    S
  }

  /// <summary>
  /// Configuration for a <see cref="MarkKilledTrigger"/>.
  /// </summary>
  [Serializable]
  public class MarkKilledTriggerConfig : TriggerConfig
  {
    #region Properties

    /// <summary>
    /// The required rank of the killed mark.
    /// </summary>
    public Rank RequiredRank { get; set; } = Rank.All;

    #endregion Properties
  }
}
