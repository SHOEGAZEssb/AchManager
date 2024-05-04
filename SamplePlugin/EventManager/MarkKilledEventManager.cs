using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;

namespace AchManager.EventManager
{
  internal class MarkKilledEventManager : AchievementUpdateEventManagerBase<MarkKilledEventArgs>
  {
    #region Singleton

    public static MarkKilledEventManager Instance
    {
      get
      {
        _instance ??= new MarkKilledEventManager();
        return _instance;
      }
    }
    private static MarkKilledEventManager? _instance;

    #endregion Singleton

    private static readonly Dictionary<uint, NotoriousMonster?> _notoriousMonstersCache = [];

    private MarkKilledEventManager()
    {
      Svc.Framework.Update += Framework_Update;
    }

    private void Framework_Update(Dalamud.Plugin.Services.IFramework framework)
    {
      // check if previous target is dead
      var prevTarget = Svc.Targets.PreviousTarget;
      if (prevTarget != null && prevTarget.IsDead)
      {
        // check if target was a mark
        if (prevTarget.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc)
        {
          var nm = GetNotoriousMonster(prevTarget.DataId);
          if (nm != null)
            FireOnEvent(null);
        }
      }
    }

    private NotoriousMonster? GetNotoriousMonster(uint dataId)
    {
      if (_notoriousMonstersCache.TryGetValue(dataId, out var nm)) return nm;

      var monster = Svc.Data.GetExcelSheet<NotoriousMonster>()?.FirstOrDefault(n => n.BNpcBase.Row == dataId);

      _notoriousMonstersCache[dataId] = monster;

      return monster;
    }
  }
}
