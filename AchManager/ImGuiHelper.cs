using ImGuiNET;

namespace AchManager
{
  /// <summary>
  /// ImGui helper functions.
  /// </summary>
  internal static class ImGuiHelper
  {
    /// <summary>
    /// Shows a tooltip for the currently hovered element.
    /// </summary>
    /// <param name="toolTip">ToolTip to show.</param>
    public static void ShowToolTip(string toolTip)
    {
      if (ImGui.IsItemHovered())
      {
        ImGui.BeginTooltip();
        ImGui.SetTooltip(toolTip);
        ImGui.EndTooltip();
      }
    }
  }
}