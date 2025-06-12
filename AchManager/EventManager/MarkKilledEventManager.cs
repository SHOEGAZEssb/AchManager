using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using System.Collections.Generic;

namespace AchManager.EventManager
{
  /// <summary>
  /// Event manager that informs when a mark is killed.
  /// </summary>
  internal class MarkKilledEventManager : AchievementUpdateEventManagerBase<MarkKilledEventArgs>
  {
    #region Singleton

    /// <summary>
    /// The unique instance of this event manager.
    /// </summary>
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

    #region Properties

    /// <summary>
    /// Cache for fetched notorious monsters from excel.
    /// </summary>
    private static readonly Dictionary<uint, NotoriousMonster> _notoriousMonstersCache = [];

    private IGameObject? _cachedTarget;

    #endregion Properties

    #region Construction

    private MarkKilledEventManager()
    {
      Svc.Framework.Update += Framework_Update;
    }

    #endregion Construction

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Dispose()
    {
      Svc.Framework.Update -= Framework_Update;
    }

    private void Framework_Update(Dalamud.Plugin.Services.IFramework framework)
    {
      // check if previous target is dead
      var prevTarget = Svc.Targets.PreviousTarget;
      if (prevTarget != null && prevTarget.IsDead && prevTarget.Address != _cachedTarget?.Address)
      {
        // check if target was a mark
        if (prevTarget.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc)
        {
          var nm = GetNotoriousMonster(prevTarget.DataId);
          if (nm.HasValue)
          {
            Svc.Log.Debug($"{nameof(MarkKilledEventManager)}: Fire ({nm.Value.BNpcName.Value.Singular})");
            _cachedTarget = prevTarget;
            FireOnEvent(new MarkKilledEventArgs(nm.Value.Rank));
          }
        }
      }
    }

    /// <summary>
    /// Gets the <see cref="NotoriousMonster"/> with the given
    /// <paramref name="dataId"/> from excel or from the <see cref="_notoriousMonstersCache"/>.
    /// </summary>
    /// <param name="dataId">ID of the <see cref="NotoriousMonster"/> to get.</param>
    /// <returns>The <see cref="NotoriousMonster"/>.
    /// Null if no <see cref="NotoriousMonster"/> for the given
    /// <paramref name="dataId"/> exists.</returns>
    private static NotoriousMonster? GetNotoriousMonster(uint dataId)
    {
      if (_notoriousMonstersCache.TryGetValue(dataId, out var nm))
        return nm;

      foreach (var monster in Svc.Data.GetExcelSheet<NotoriousMonster>())
      {
        if (monster.BNpcBase.RowId == dataId)
        {
          _notoriousMonstersCache[dataId] = monster;
          return monster;
        }
      }

      return null;
    }
  }
}
