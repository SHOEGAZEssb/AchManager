using AchManager.EventManager;
using Newtonsoft.Json;

namespace AchManager.AchievementTrigger
{
  internal class BannerShownTrigger : AchievementUpdateTriggerBase
  {
    #region Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string TriggerIdentifier => nameof(BannerShownTrigger);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public override TriggerConfig Config => TypedConfig;

    /// <summary>
    /// The specific configuration for this trigger.
    /// </summary>
    public BannerShownTriggerConfig TypedConfig { get; } = new BannerShownTriggerConfig();

    #endregion Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Dispose()
    {
      BannerShownEventManager.Instance.OnEvent -= BannerShownEventManager_OnEvent;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void Init()
    {
      if (_isInitialized)
        return;

      BannerShownEventManager.Instance.OnEvent += BannerShownEventManager_OnEvent; ;
      _isInitialized = true;
    }

    private void BannerShownEventManager_OnEvent(object? sender, BannerShownEventArgs e)
    {
      if (TypedConfig.Banners.Contains(e.BannerID))
        FireOnTrigger();
    }
  }
}
