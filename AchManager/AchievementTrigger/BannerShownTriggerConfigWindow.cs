using AchManager.Windows;
using ImGuiNET;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Configuration window for a <see cref="BannerShownTrigger"/>.
  /// </summary>
  /// <param name="config">The configuration of the trigger.</param>
  /// <param name="name">Name of the window.</param>
  /// <param name="flags">ImGui flags for the window.</param>
  /// <param name="forceMainWindow">If the window should be treated as a main window.</param>
  internal sealed class BannerShownTriggerConfigWindow(BannerShownTriggerConfig config, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false)
    : DefaultTriggerConfigWindow(config, name, flags, forceMainWindow)
  {
    #region Properties

    private readonly BannerShownTriggerConfig _config = config;
    private bool _showBannerIDs = true;

    #endregion Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void DrawChildContent()
    {
      ImGui.Checkbox("Show banner IDs", ref _showBannerIDs);
      ImGuiHelper.ShowToolTip("If enabled, shows the internal banner IDs.");

      ImGui.Separator();

      ImGui.BeginGroup();
      var selectedBanners = _config.Banners;
      if (ImGui.BeginTable("BannerTable", 2, ImGuiTableFlags.BordersInnerV))
      {
        ImGui.TableNextRow();

        int i = 0;
        foreach (var kvp in BannerShownTriggerConfig.AvailableBanners)
        {
          // new row every two items
          if (i % 2 == 0 && i != 0)
            ImGui.TableNextRow();

          ImGui.TableSetColumnIndex(i % 2);

          var isSelected = selectedBanners.Contains(kvp.Key);
          if (ImGui.Checkbox(_showBannerIDs ? $"{kvp.Value} ({kvp.Key})" : kvp.Value, ref isSelected))
          {
            if (isSelected)
              _config.Banners.Add(kvp.Key);
            else
              _config.Banners.Remove(kvp.Key);

            Plugin.Configuration.Save();
          }

          i++;
        }
      }
      ImGui.EndTable();
      ImGui.EndGroup();
      ImGuiHelper.ShowToolTip("List of banners that will cause this trigger to fire.");
    }
  }
}