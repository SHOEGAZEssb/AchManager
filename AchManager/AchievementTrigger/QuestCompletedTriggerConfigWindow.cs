using AchManager.Windows;
using ImGuiNET;
using System;
using System.Linq;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Configuration window for a <see cref="QuestCompletedTrigger"/>.
  /// </summary>
  /// <param name="config">The configuration of the trigger.</param>
  /// <param name="pluginConfig">The current plugin configuration.</param>
  /// <param name="name">Name of the window.</param>
  /// <param name="flags">ImGui flags for the window.</param>
  /// <param name="forceMainWindow">If the window should be treated as a main window.</param>
  internal class QuestCompletedTriggerConfigWindow(QuestCompletedTriggerConfig config, Configuration pluginConfig, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false)
    : DefaultTriggerConfigWindow(config, pluginConfig, name, flags, forceMainWindow)
  {
    #region Properties

    private readonly QuestCompletedTriggerConfig _config = config;

    #endregion Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void DrawChildContent()
    {
      ImGui.Text("Required quest type: ");
      ImGui.SameLine();

      var enumValues = Enum.GetValues<RequiredQuestType>();
      var index = Array.IndexOf(enumValues, _config.RequiredQuestType);
      if (ImGui.Combo("##requiredQuestTypeCB", ref index, [.. enumValues.Select(r => r.ToString())], enumValues.Length))
      {
        _config.RequiredQuestType = enumValues[index];
        _pluginConfig.Save();
      }
    }
  }
}