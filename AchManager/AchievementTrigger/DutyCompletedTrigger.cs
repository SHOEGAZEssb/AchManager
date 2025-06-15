using AchManager.EventManager;
using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Trigger that triggers when a duty is completed.
  /// </summary>
  [Serializable]
  public sealed class DutyCompletedTrigger : AchievementUpdateTriggerBase
  {
    #region Properties

    private static readonly ContentFinderCondition[] _duties = Svc.Data.GameData.GetExcelSheet<ContentFinderCondition>()?.ToArray() ?? [];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string TriggerIdentifier => nameof(DutyCompletedTrigger);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public override TriggerConfig Config => TypedConfig;

    /// <summary>
    /// The specific configuration for this trigger.
    /// </summary>
    public DutyCompletedTriggerConfig TypedConfig { get; } = new DutyCompletedTriggerConfig();

    #endregion Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Dispose()
    {
      DutyCompletedEventManager.Instance.OnEvent -= DutyCompletedEventManager_OnEvent;
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void Init()
    {
      if (_isInitialized)
        return;

      DutyCompletedEventManager.Instance.OnEvent += DutyCompletedEventManager_OnEvent;
      _isInitialized = true;
    }

    private void DutyCompletedEventManager_OnEvent(object? sender, DutyCompletedEventArgs e)
    {
      if (TypedConfig.RequiredContentTypes.Contains(ContentType.All)
          || TypedConfig.RequiredContentTypes.Select(t => (uint)t).Contains(_duties.First(d => d.TerritoryType.RowId == e.TerritoryID).ContentType.RowId))
        FireOnTrigger();
    }
  }
}