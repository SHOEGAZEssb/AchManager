using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace AchManager
{

    internal unsafe class FateTrigger : AchievementUpdateTrigger
    {
        private FateContext* lastFate;

        public FateTrigger()
        {
            Svc.Framework.Update += Framework_Update;
        }

        private void Framework_Update(Dalamud.Plugin.Services.IFramework framework)
        {
            FateContext* currentFate = null;
            var fm = FateManager.Instance();
            if (fm->CurrentFate != null && PlayerState.Instance()->IsLevelSynced == 1)
            {
                currentFate = fm->CurrentFate;
            }

            if (lastFate != currentFate)
                FateChanged(currentFate);

            lastFate = currentFate;
        }

        private void FateChanged(FateContext* newFate)
        {
            if (newFate != null)
            {
                Svc.Log.Debug($"Fate changed to {newFate->Name}");
            }
            else
                FireOnTrigger();
            Svc.Log.Debug($"Fate changed to null");
        }
    }
}
