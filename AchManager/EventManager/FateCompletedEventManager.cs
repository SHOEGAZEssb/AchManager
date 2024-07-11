using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Threading;

namespace AchManager.EventManager
{

  internal unsafe class FateCompletedEventManager : AchievementUpdateEventManagerBase<EventArgs>
  {
    #region Singleton

    public static FateCompletedEventManager Instance
    {
      get
      {
        _instance ??= new FateCompletedEventManager();
        return _instance;
      }
    }
    private static FateCompletedEventManager? _instance;

    #endregion Singleton

    #region Properties

    private FateContext* _lastFate;

    #endregion Properties

    #region Construction

    private FateCompletedEventManager()
    {
      Svc.Framework.Update += Framework_Update;
    }

    #endregion Construction

    public override void Dispose()
    {
      Svc.Framework.Update -= Framework_Update;
    }

    private void Framework_Update(Dalamud.Plugin.Services.IFramework framework)
    {
      FateContext* currentFate = null;
      var fm = FateManager.Instance();
      if (fm->CurrentFate != null && PlayerState.Instance()->IsLevelSynced == 1)
      {
        currentFate = fm->CurrentFate;
      }

      if (_lastFate != currentFate)
        FateChanged(currentFate);

      _lastFate = currentFate;
    }

    private void FateChanged(FateContext* newFate)
    {
      if (newFate == null)
      {
        Svc.Log.Debug($"{nameof(FateCompletedEventManager)}: Fire");
        FireOnEvent(EventArgs.Empty);
      }
    }
  }
}
