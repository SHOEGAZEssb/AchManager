using ECommons.DalamudServices;
using ECommons.EzHookManager;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AchManager.EventManager
{
  internal sealed unsafe class BannerShownEventManager : AchievementUpdateEventManagerBase<BannerShownEventArgs>
  {
    #region Singleton

    /// <summary>
    /// The unique instance of this event manager.
    /// </summary>
    public static BannerShownEventManager Instance
    {
      get
      {
        _instance ??= new BannerShownEventManager();
        return _instance;
      }
    }
    private static BannerShownEventManager? _instance;

    #endregion Singleton

    #region Properties

    public int LastSeenBannerID { get; private set; } = -1;

    private delegate void ImageSetImageTextureDelegate(AtkUnitBase* addon, int bannerId, int a3, int sfxId);
    private readonly EzHook<ImageSetImageTextureDelegate> SetImageTextureHook;

    #endregion Properties

    #region Construction

    public BannerShownEventManager()
    {
      SetImageTextureHook = new EzHook<ImageSetImageTextureDelegate>("48 89 5C 24 ?? 57 48 83 EC 30 48 8B D9 89 91", OnSetImageTexture);
      SetImageTextureHook.Enable();
    }

    #endregion Construction

    public override void Dispose()
    {
      SetImageTextureHook.Disable();
    }

    private void OnSetImageTexture(AtkUnitBase* addon, int bannerId, int a3, int soundEffectId)
    {
      LastSeenBannerID = bannerId;
      Svc.Log.Debug($"{nameof(BannerShownEventManager)}: Fire ({bannerId})");
      FireOnEvent(new BannerShownEventArgs(bannerId));
      SetImageTextureHook.Original(addon, bannerId, a3, soundEffectId);
    }
  }
}
