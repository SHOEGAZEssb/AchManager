using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AchManager.AchievementTrigger
{
  public enum ContentType
  {
    All = 0,
    Dungeons = 2, // dungeons
    Guildhests = 3, // guildhests
    Trials = 4, // trials
    PVP = 6, // pvp
    Raids = 5, // raids
    TreasureHunt = 9, // treasure hunt
    DOL = 16, // disciples of land (ocean fishing)
    GoldSaucer = 19, // gold saucer
    DeepDungeons = 21, // deep dungeons
    UltimateRaids = 28, // ultimate raids
    VC = 30 // V&C
  }

  /// <summary>
  /// Configuration for a <see cref="DutyCompletedTrigger"/>.
  /// </summary>
  [Serializable]
  public sealed class DutyCompletedTriggerConfig : TriggerConfig
  {
    #region Properties

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public List<ContentType> RequiredContentTypes { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    #endregion Properties

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      if (RequiredContentTypes == null || RequiredContentTypes.Count == 0)
        RequiredContentTypes = [ContentType.All];
    }
  }
}
