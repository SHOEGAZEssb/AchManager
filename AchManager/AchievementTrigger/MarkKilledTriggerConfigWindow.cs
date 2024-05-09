using AchManager.Windows;
using ECommons;
using ImGuiNET;
using System;
using System.Linq;

namespace AchManager.AchievementTrigger
{
  internal class MarkKilledTriggerConfigWindow(MarkKilledTriggerConfig config, Configuration pluginConfig, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false)
    : DefaultConfigWindow(config, pluginConfig, name, flags, forceMainWindow)
  {
    #region Properties

    public Rank RequiredRank
    {
      get => _config.RequiredRank;
      set
      {
        if (RequiredRank != value) 
        { 
          _config.RequiredRank = value;
          _pluginConfig.Save();
        }
      }
    }
    private readonly MarkKilledTriggerConfig _config = config;

    #endregion Properties

    protected override void DrawChildContent()
    {
      ImGui.Text("Killed rank to update: ");
      ImGui.SameLine();

      var enumValues = Enum.GetValues<Rank>();
      var index = Array.IndexOf(enumValues, RequiredRank);
      if (ImGui.Combo("##mktConfigRank", ref index, enumValues.Select(r => r.ToString()).ToArray(), enumValues.Length))
        RequiredRank = enumValues[index];
    }
  }
}
