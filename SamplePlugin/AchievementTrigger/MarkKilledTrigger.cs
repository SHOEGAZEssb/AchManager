using AchManager.EventManager;
using System;

namespace AchManager.AchievementTrigger
{
  [Serializable]
  public class MarkKilledTrigger : AchievementUpdateTriggerBase
  {
    public override string TriggerIdentifier => nameof(MarkKilledTrigger);

    #region Properties

    public MarkKilledTriggerConfig Config { get; } = new MarkKilledTriggerConfig();

    #endregion Properties

    #region Construction

    public MarkKilledTrigger()
    {
      MarkKilledEventManager.Instance.OnEvent += Instance_OnTrigger;
    }

    #endregion Construction

    public override void Dispose()
    {
      MarkKilledEventManager.Instance.OnEvent -= Instance_OnTrigger;
    }

    private void Instance_OnTrigger(object? sender, MarkKilledEventArgs e)
    {
      if (Config.RequiredRank == Rank.All || e.Rank.ToString() == Config.RequiredRank.ToString())
        FireOnTrigger();
    }
  }
}
