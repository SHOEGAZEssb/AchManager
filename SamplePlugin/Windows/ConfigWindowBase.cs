using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;

namespace AchManager.Windows
{
  internal abstract class ConfigWindowBase(Configuration pluginConfig, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false)
    : Window(name, flags, forceMainWindow)
  {
    protected Configuration _pluginConfig = pluginConfig;
  }
}