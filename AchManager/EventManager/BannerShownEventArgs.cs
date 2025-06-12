using System;

namespace AchManager.EventManager
{
  internal sealed class BannerShownEventArgs(int bannerID) : EventArgs
  {
    #region Properties

    public int BannerID { get; } = bannerID;

    #endregion Properties
  }
}