using AchManager.Windows;
using ImGuiNET;

namespace AchManager.AchievementTrigger
{
  internal class ChatMessageTriggerConfigWindow(ChatMessageTriggerConfig config, Configuration pluginConfig, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false)
    : DefaultConfigWindow(config, pluginConfig, name, flags, forceMainWindow)
  {
    #region Properties

    private readonly ChatMessageTriggerConfig _config = config;

    #endregion Properties

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
