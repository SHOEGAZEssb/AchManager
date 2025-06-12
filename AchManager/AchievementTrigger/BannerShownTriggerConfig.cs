using System.Collections.Generic;

namespace AchManager.AchievementTrigger
{
  internal sealed class BannerShownTriggerConfig : TriggerConfig
  {
    public static readonly Dictionary<int, string> AvailableBanners = new()
    {
      { 120031, "Levequest Accepted" },
      { 120032, "Levequest Complete" },
      { 120055, "Delivery Complete" },
      { 120081, "FATE Joined" },
      { 120082, "FATE Complete" },
      { 120083, "FATE Failed" },
      { 120084, "FATE Joined EXP BONUS" },
      { 120085, "FATE Complete EXP BONUS" },
      { 120086, "FATE Failed EXP BONUS" },
      { 120093, "Treasure Obtained!" },
      { 120094, "Treasure Found!" },
      { 120095, "Venture Commenced!" },
      { 120096, "Venture Accomplished!" },
      { 120141, "Voyage Commenced" },
      { 120142, "Voyage Complete" },
      { 121081, "Tribal Quest Accepted" },
      { 121082, "Tribal Quest Complete" },
      { 121561, "GATE Joined" },
      { 121562, "GATE Complete" },
      { 121563, "GATE Failed" },
      { 128370, "Stellar Mission Commenced" },
      { 128371, "Stellar Mission Abandoned" },
      { 128372, "Stellar Mission Failed" },
      { 128373, "Stellar Mission Complete" }
    };

    #region Properties

    public List<int> Banners { get; } = [];

    #endregion Properties
  }
}
