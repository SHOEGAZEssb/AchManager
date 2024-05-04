using AchManager.EventManager;
using System;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Trigger that fires when a mark is killed.
  /// </summary>
  [Serializable]
  public class MarkKilledTrigger : AchievementUpdateTriggerBase
  {
    public override string TriggerIdentifier => nameof(MarkKilledTrigger);

    #region Properties

    /// <summary>
    /// Configuration for this trigger.
    /// </summary>
    public MarkKilledTriggerConfig Config { get; } = new MarkKilledTriggerConfig();

    #endregion Properties

    /// <summary>
    /// Disposes this trigger.
    /// Unlinks from the <see cref="MarkKilledEventManager"/>.
    /// </summary>
    public override void Dispose()
    {
      MarkKilledEventManager.Instance.OnEvent -= Instance_OnTrigger;
      GC.SuppressFinalize(this);
    }

    protected override void Init()
    {
      MarkKilledEventManager.Instance.OnEvent += Instance_OnTrigger;
    }

    private void Instance_OnTrigger(object? sender, MarkKilledEventArgs e)
    {
      if (Config.RequiredRank == Rank.All || e.Rank.ToString() == Config.RequiredRank.ToString())
        FireOnTrigger();
    }
  }
}
