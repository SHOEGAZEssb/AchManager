using ECommons.DalamudServices;
using ECommons.EzHookManager;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;

namespace AchManager
{
  internal unsafe static class AchievementHookManager
  {
    #region Properties

    public static event EventHandler<AchievementProgressEventArgs>? OnAchievementProgress;

    private delegate void ReceiveAchievementProgressDelegate(Achievement* achievement, uint id, uint current, uint max);
    private static readonly EzHook<ReceiveAchievementProgressDelegate> ReceiveAchievementProgressHook;

    #endregion Properties

    #region Construction

    static AchievementHookManager()
    {
      ReceiveAchievementProgressHook = new EzHook<ReceiveAchievementProgressDelegate>(Achievement.Addresses.ReceiveAchievementProgress.String, ReceiveAchievementProgressDetour);
      ReceiveAchievementProgressHook.Enable();
    }

    #endregion Construction

    public static void RequestProgess(uint id)
    {
      Achievement.Instance()->RequestAchievementProgress(id);
    }

    private static void ReceiveAchievementProgressDetour(Achievement* achievement, uint id, uint current, uint max)
    {
      Svc.Log.Debug($"Receive Achievement ({id}) progress: {current}/{max}");
      OnAchievementProgress?.Invoke(null, new AchievementProgressEventArgs(id, current, max));
      ReceiveAchievementProgressHook.Original(achievement, id, current, max);
    }
  }
}
