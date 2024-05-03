using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Linq;

namespace AchManager
{
    internal class ContextMenuManager
    {
        private readonly IContextMenu contextMenu;

        public ContextMenuManager(IContextMenu contextMenuService)
        {
            contextMenu = contextMenuService;
            contextMenu.OnMenuOpened += ContextMenu_OnMenuOpened;
        }

        private void ContextMenu_OnMenuOpened(Dalamud.Game.Gui.ContextMenu.MenuOpenedArgs args)
        {
            if (args.AddonName == "Achievement")
            {
                var mi = new MenuItem()
                {
                    Name = "Test",
                    PrefixChar = 'A',
                    OnClicked = OnMenuItemClicked
                };
                args.AddMenuItem(mi);

                unsafe
                {
                    var o = (AgentInterface*)args.AgentPtr;
                    var a = (AgentContext*)args.AgentPtr;
                    var b = (Achievement*)args.AgentPtr;
                    var c = (AchievementListModule*)args.AgentPtr;

                    var d = (AchievementListModule*)args.AddonPtr;
                    var e = (Achievement*)args.AddonPtr;
                    var f = (AddonContextMenu*)args.AddonPtr;

                    var ifa = (AgentInterface*)args.EventInterfaces.FirstOrDefault();
                    var context = (AgentContext*)args.EventInterfaces.FirstOrDefault();
                    var ach = (Achievement*)args.EventInterfaces.FirstOrDefault();
                    var ach2 = (AchievementListModule*)args.EventInterfaces.FirstOrDefault();
                }
            }
        }

        private void OnMenuItemClicked(MenuItemClickedArgs args)
        {
            unsafe
            {
                try
                {
                    var o = (AgentInterface*)args.AgentPtr;
                    var a = (AgentContext*)args.AgentPtr;
                    var b = (Achievement*)args.AgentPtr;
                    var c = (AchievementListModule*)args.AgentPtr;

                    var d = (AchievementListModule*)args.AddonPtr;
                    var e = (Achievement*)args.AddonPtr;
                    var f = (AddonContextMenu*)args.AddonPtr;

                    var ifa = (AgentInterface*)args.EventInterfaces.FirstOrDefault();
                    var context = (AgentContext*)args.EventInterfaces.FirstOrDefault();
                    var ach = (Achievement*)args.EventInterfaces.FirstOrDefault();
                    var ach2 = (AchievementListModule*)args.EventInterfaces.FirstOrDefault();
                }
                catch
                {

                }

            }
        }
    }

}
