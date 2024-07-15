using AchManager.Windows;
using ImGuiNET;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Configuration window for a <see cref="ChatMessageTriggerConfig"/>.
  /// </summary>
  /// <param name="config">The configuration of the trigger.</param>
  /// <param name="pluginConfig">The current plugin configuration.</param>
  /// <param name="name">Name of the window.</param>
  /// <param name="flags">ImGui flags for the window.</param>
  /// <param name="forceMainWindow">If the window should be treated as a main window.</param>
  internal class ChatMessageTriggerConfigWindow(ChatMessageTriggerConfig config, Configuration pluginConfig, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false)
    : DefaultConfigWindow(config, pluginConfig, name, flags, forceMainWindow)
  {
    #region Properties

    private readonly ChatMessageTriggerConfig _config = config;

    #endregion Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void DrawChildContent()
    {
      var isRegex = _config.IsRegex;

      if (isRegex)
        ImGui.Text("Regex pattern to check message against:");
      else
        ImGui.Text("Required Message Content:");

      var rmq = _config.RequiredMessageContent;
      if (ImGui.InputText("##requiredMessageContent", ref rmq, 128))
      {
        _config.RequiredMessageContent = rmq;
        _pluginConfig.Save();
      }

      ImGui.SameLine();
      if (ImGui.Checkbox("Is Regex##isRegexCB", ref isRegex))
      {
        _config.IsRegex = isRegex;
        _pluginConfig.Save();
      }
    }
  }
}
