using AchManager.AchievementTrigger;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;

namespace AchManager.Windows
{
  internal class DefaultConfigWindow(TriggerConfig triggerConfig, Configuration pluginConfig, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false)
    : Window(name, flags, forceMainWindow)
  {
    private readonly TriggerConfig _triggerConfig = triggerConfig;
    protected readonly Configuration _pluginConfig = pluginConfig;

    /// <summary>
    /// Draws the config.
    /// Inheriting config windows should not override
    /// this, but override <see cref="DrawChildContent"/>
    /// instead.
    /// </summary>
    public override void Draw()
    {
      if (ImGui.TreeNodeEx("##notificationConfig", ImGuiTreeNodeFlags.CollapsingHeader | ImGuiTreeNodeFlags.DefaultOpen, "Progress Notifications"))
      {
        ImGui.Indent();

        ImGui.Text("Chat");
        ImGui.SameLine();
        var chat = _triggerConfig.ShowChatMessage;
        if (ImGui.Checkbox("##showChatMessage", ref chat))
        {
          _triggerConfig.ShowChatMessage = chat;
          _pluginConfig.Save();
        }

        ImGui.Text("Dalamud Notification");
        ImGui.SameLine();
        var notif = _triggerConfig.ShowNotification;
        if (ImGui.Checkbox("##showNotification", ref notif))
        {
          _triggerConfig.ShowNotification = notif;
          _pluginConfig.Save();
        }

        ImGui.Unindent();
      }

      if (ImGui.TreeNodeEx("##delayConfig", ImGuiTreeNodeFlags.CollapsingHeader | ImGuiTreeNodeFlags.DefaultOpen, "Progress Check Delay"))
      {
        ImGui.Indent();

        ImGui.Text("Delay (ms)");
        ImGui.SameLine();
        var delay = _triggerConfig.DelayMS;
        if (ImGui.InputInt("##delayMS", ref delay) && delay >= 0)
        {
          _triggerConfig.DelayMS = delay;
          _pluginConfig.Save();
        }

        ImGui.Unindent();
      }

      DrawChildContent();
    }

    protected virtual void DrawChildContent() { }
  }
}