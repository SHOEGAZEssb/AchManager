using AchManager.Windows;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      ImGui.Text("Required Message Content: ");
      var rmq = _config.RequiredMessageContent;
      if (ImGui.InputText("##requiredMessageContent", ref rmq, 128))
      {
        _config.RequiredMessageContent = rmq;
        _pluginConfig.Save();
      }
    }
  }
}
