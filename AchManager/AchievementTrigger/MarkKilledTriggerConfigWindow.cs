using AchManager.Windows;
using ImGuiNET;
using System;
using System.Linq;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Configuration window for a <see cref="MarkKilledTriggerConfig"/>.
  /// </summary>
  /// <param name="config">The configuration of the trigger.</param>
  /// <param name="name">Name of the window.</param>
  /// <param name="flags">ImGui flags for the window.</param>
  /// <param name="forceMainWindow">If the window should be treated as a main window.</param>
  internal class MarkKilledTriggerConfigWindow(MarkKilledTriggerConfig config, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false)
    : DefaultTriggerConfigWindow(config, name, flags, forceMainWindow)
  {
    #region Properties

    private readonly MarkKilledTriggerConfig _config = config;

    #endregion Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void DrawChildContent()
    {
      ImGui.BeginGroup();
      ImGui.Text("Killed rank to update: ");
      ImGui.SameLine();

      var enumValues = Enum.GetValues<Rank>();
      var index = Array.IndexOf(enumValues, _config.RequiredRank);
      if (ImGui.Combo("##mktConfigRank", ref index, [.. enumValues.Select(r => r.ToString())], enumValues.Length))
      {
        _config.RequiredRank = enumValues[index];
        Plugin.Configuration.Save();
      }
      ImGui.EndGroup();
      if (ImGui.IsItemHovered())
      {
        ImGui.BeginTooltip();
        ImGui.SetTooltip("Defines which rank the killed mark needs to have in order for the trigger to fire.");
        ImGui.EndTooltip();
      }
    }
  }
}
