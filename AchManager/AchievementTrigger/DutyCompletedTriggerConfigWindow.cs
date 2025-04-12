using AchManager.Windows;
using ImGuiNET;
using System;
using System.Collections.Generic;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Configuration window for a <see cref="DutyCompletedTriggerConfig"/>.
  /// </summary>
  /// <param name="config">The configuration of the trigger.</param>
  /// <param name="pluginConfig">The current plugin configuration.</param>
  /// <param name="name">Name of the window.</param>
  /// <param name="flags">ImGui flags for the window.</param>
  /// <param name="forceMainWindow">If the window should be treated as a main window.</param>
  internal class DutyCompletedTriggerConfigWindow(DutyCompletedTriggerConfig config, Configuration pluginConfig, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false)
    : DefaultTriggerConfigWindow(config, pluginConfig, name, flags, forceMainWindow)
  {
    #region Properties

    private readonly DutyCompletedTriggerConfig _config = config;

    #endregion Properties

    #region Construction

    #endregion Construction

    protected override void DrawChildContent()
    {
      ImGui.BeginGroup();
      ImGui.Text("Required Duty Type:");

      if (_config.RequiredContentTypes == null || _config.RequiredContentTypes.Count == 0)
        _config.RequiredContentTypes = [ContentType.All];

      var selectedTypes = new HashSet<ContentType>(_config.RequiredContentTypes);

      bool changed = false;

      foreach (var type in Enum.GetValues<ContentType>())
      {
        bool isChecked = selectedTypes.Contains(type);
        if (ImGui.Checkbox(type.ToString(), ref isChecked))
        {
          changed = true;

          if (isChecked)
          {
            if (type == ContentType.All)
            {
              selectedTypes.Clear();
              selectedTypes.Add(ContentType.All);
            }
            else
            {
              selectedTypes.Remove(ContentType.All);
              selectedTypes.Add(type);
            }
          }
          else
          {
            selectedTypes.Remove(type);
            if (selectedTypes.Count == 0)
              selectedTypes.Add(ContentType.All);
          }
        }
      }

      if (changed)
      {
        _config.RequiredContentTypes = [.. selectedTypes];
        _pluginConfig.Save();
      }

      ImGui.EndGroup();
      if (ImGui.IsItemHovered())
      {
        ImGui.BeginTooltip();
        ImGui.SetTooltip("Defines which type of duty must have been completed in order for the trigger to fire.");
        ImGui.EndTooltip();
      }
    }
  }
}
