using AchManager.Windows;
using Dalamud.Bindings.ImGui;
using System;
using System.Linq;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Configuration window for a <see cref="QuestCompletedTrigger"/>.
  /// </summary>
  /// <param name="config">The configuration of the trigger.</param>
  /// <param name="name">Name of the window.</param>
  /// <param name="flags">ImGui flags for the window.</param>
  /// <param name="forceMainWindow">If the window should be treated as a main window.</param>
  internal sealed class QuestCompletedTriggerConfigWindow(QuestCompletedTriggerConfig config, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false)
    : DefaultTriggerConfigWindow(config, name, flags, forceMainWindow)
  {
    #region Properties

    private readonly QuestCompletedTriggerConfig _config = config;

    #endregion Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void DrawChildContent()
    {
      ImGui.BeginGroup();
      ImGui.Text("Required quest type: ");
      ImGui.SameLine();

      var enumValues = Enum.GetValues<RequiredQuestType>();
      var index = Array.IndexOf(enumValues, _config.RequiredQuestType);
      if (ImGui.Combo("##requiredQuestTypeCB", ref index, [.. enumValues.Select(r => r.ToString())], enumValues.Length))
      {
        _config.RequiredQuestType = enumValues[index];
        Plugin.Configuration.Save();
      }
      ImGui.EndGroup();
      if (ImGui.IsItemHovered())
      {
        ImGui.BeginTooltip();
        ImGui.SetTooltip("Defines which type of quest needs to be completed in order for the trigger to fire.");
        ImGui.EndTooltip();
      }
    }
  }
}