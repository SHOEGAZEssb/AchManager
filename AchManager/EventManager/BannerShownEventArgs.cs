using System;

namespace AchManager.EventManager
{
  /// <summary>
  /// EventArgs for the <see cref="BannerShownEventManager"/>.
  /// </summary>
  /// <param name="bannerID"></param>
  internal sealed class BannerShownEventArgs(int bannerID) : EventArgs
  {
    #region Properties

    /// <summary>
    /// ID of the shown banner.
    /// </summary>
    public int BannerID { get; } = bannerID;

    #endregion Properties
  }
}