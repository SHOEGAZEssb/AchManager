using AchManager.Windows;
using ImGuiNET;
using System;
using System.Linq;

namespace AchManager.AchievementTrigger
{
  internal class QuestCompletedTriggerConfigWindow(QuestCompletedTriggerConfig config, Configuration pluginConfig, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false)
    : DefaultConfigWindow(config, pluginConfig, name, flags, forceMainWindow)
  {
    #region Properties

    private readonly QuestCompletedTriggerConfig _config = config;

    #endregion Properties

    protected override void DrawChildContent()
    {
      ImGui.Text("Required quest type: ");
      ImGui.SameLine();

      var enumValues = Enum.GetValues<RequiredQuestType>();
      var index = Array.IndexOf(enumValues, _config.RequiredQuestType);
      if (ImGui.Combo("##requiredQuestTypeCB", ref index, enumValues.Select(r => r.ToString()).ToArray(), enumValues.Length))
      {
        _config.RequiredQuestType = enumValues[index];
        _pluginConfig.Save();
      }
    }
  }
}
