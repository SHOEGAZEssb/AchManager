﻿using AchManager.EventManager;
using System;

namespace AchManager.AchievementTrigger
{
  [Serializable]
  public class DutyCompletedTrigger : AchievementUpdateTriggerBase
  {
    public override string TriggerIdentifier => nameof(DutyCompletedTrigger);

    #region Construction

    public DutyCompletedTrigger()
    {
      DutyCompletedEventManager.Instance.OnEvent += Instance_OnTrigger;
    }

    #endregion Construction

    public override void Dispose()
    {
      DutyCompletedEventManager.Instance.OnEvent -= Instance_OnTrigger;
    }

    private void Instance_OnTrigger(object? sender, EventArgs e)
    {
      FireOnTrigger();
    }
  }
}
